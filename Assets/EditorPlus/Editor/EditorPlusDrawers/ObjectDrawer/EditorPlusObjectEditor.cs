using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {

    /// <summary>
    /// By implementing this interface, one can add a decorator to any class.
    /// It allows you to add elements before or after objects in the inspector.
    /// <br /><br />
    /// it will be used both on the editor target (<see cref="ScriptableObject"/>,
    /// <see cref="MonoBehaviour"/> etc.), but also around object fields.
    /// </summary>
    /// <seealso cref="DecoratorBase&lt;Attr&gt;"/>
    public interface IClassDecorator {
        string TargetPropertyPath { set; }
        void OnEnable(List<object> targets);
        float GetHeight(List<object> targets);
        Rect OnInspectorGUIBefore(Rect rect, List<object> targets);
        Rect OnInspectorGUIAfter(Rect rect, List<object> targets);
    }
    
#if !EDITOR_PLUS_DISABLE_EDITOR
    /// <summary>
    /// This class is the main object editor for the plugin. It draws
    /// everything using the <see cref="SerializedPropertyDrawer" />.
    /// </summary>
    [CustomEditor(typeof(Object), true)]
    [CanEditMultipleObjects]
    public class EditorPlusObjectEditor : UnityEditor.Editor {
        private SerializedPropertyDrawer Drawer;

        public override void OnInspectorGUI() {
            serializedObject.Update();

            SerializedProperty property = serializedObject.GetIterator();
            
            if (Drawer is null)
                Drawer = new SerializedPropertyDrawer();

            Rect controlRect = EditorGUILayout.GetControlRect(true, Drawer.GetPropertyHeight(property, false));
            Drawer.Draw(controlRect, property, false);

            
            // Prevent error if the target has been destroyed during OnInspectorGUI 
            if ((bool)serializedObject.targetObject)
                serializedObject.ApplyModifiedProperties();
        }
    }
    
    /// <summary>
    /// This class is the main property drawer for the plugin. If the property is
    /// of <see cref="SerializedPropertyType.Generic">generic</see> type, it draws
    /// everything using the <see cref="SerializedPropertyDrawer"/>. Otherwise it draws
    /// the field normally using the <see cref="EditorGUI.PropertyField(Rect, SerializedProperty, GUIContent)"/> method.
    /// <br /><br />
    /// This allows for generic fields to be displayed with class decorators,
    /// and to be drawn with the better Editor+ lists. 
    /// </summary>
    [CustomPropertyDrawer(typeof(object), true)]
    public class EditorPlusObjectPropertyDrawer : PropertyDrawer {
        
        private SerializedPropertyDrawer Drawer = new SerializedPropertyDrawer();
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.Generic) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            Drawer.Draw(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.Generic) {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            
            return Drawer.GetPropertyHeight(property, label);
        }
    }

#endif
    
    
    /// <summary>
    /// This class is responsible for drawing a  generic <see cref="SerializedProperty"/> representing any object.
    /// It can be an inner generic property or a "root" object like a <see cref="ScriptableObject"/> for example.
    /// It will handle <see cref="IClassDecorator">class decorators</see>, <see cref="DecoratorBase&lt;Attr&gt;">decorators</see>
    /// and custom list drawing.
    /// </summary>
    public class SerializedPropertyDrawer {

        private readonly float FieldMargin = EditorGUIUtility.standardVerticalSpacing;
        private ListDrawer ListDrawerInstance;
        private List<IClassDecorator> _classDecoratorList;
        private List<Decorator> _regularDecoratorList;
        private List<object> _targetList;

        private List<IClassDecorator> GetClassDecorators(SerializedProperty property) {
            if (_classDecoratorList != null)
                return _classDecoratorList;

            _classDecoratorList = TypeUtils.GetAllTypesImplementing(typeof(IClassDecorator))
                .Select(TypeUtils.CreateInstance<IClassDecorator>)
                .ToList();
            
            List<object> targetList = GetTargets(property);
            
            foreach (var classDecorator in _classDecoratorList) {
                classDecorator.TargetPropertyPath = property.propertyPath;
                classDecorator.OnEnable(targetList);
            }
            
            return _classDecoratorList;
        }
        
        private List<Decorator> GetRegularDecorators(SerializedProperty property) {
            if (_regularDecoratorList != null)
                return _regularDecoratorList;

            try {
                EditorUtils.GetMemberInfo(property, out _, out var memberInfo);
                _regularDecoratorList = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(memberInfo);
            }
            catch (Exception) {
                _regularDecoratorList = new List<Decorator>();
            }

            return _regularDecoratorList;
        }

        private List<object> GetTargets(SerializedProperty property) {
            if (_targetList != null)
                return _targetList;
            
            _targetList = new List<object>();

            if (string.IsNullOrEmpty(property.propertyPath)) {
                _targetList = property.serializedObject.targetObjects.Select(obj => (object) obj).ToList();
            }
            else {
                foreach (var masterParentTarget in property.serializedObject.targetObjects) {
                    EditorUtils.GetMemberInfo(masterParentTarget, property, out var parentObject, out var parentMemberInfo);
                    _targetList.Add(EditorUtils.GetGeneralValue<object>(parentObject, parentMemberInfo));
                }
            }


            return _targetList;
        }

        private ListDrawer GetListDrawer(SerializedProperty property) {
            if (ListDrawerInstance is null) {
                ListDrawerInstance = new ListDrawer(property);
            }

            return ListDrawerInstance;
        }

        
        public float GetPropertyHeight(SerializedProperty property, bool showLabel = true) {
            return GetPropertyHeight(property, showLabel ? new GUIContent(property.displayName) : GUIContent.none);
        }
        
        public float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float fieldHeight = 0;
            
            List<Decorator> regularDecorators = GetRegularDecorators(property);
            if (regularDecorators.All(d => d.ShowProperty(property))) {

                fieldHeight = MustBeShown(label) ? EditorGUIUtility.singleLineHeight : 0;

                if (property.isExpanded) {
                    SerializedProperty nextProperty = property.Copy();
                    int startDepth = property.depth;
                    if (nextProperty.NextVisible(true)) {
                        do {
                            fieldHeight += GetInnerPropertyHeight(nextProperty);
                        } while (nextProperty.NextVisible(false) && startDepth < nextProperty.depth);
                    }

                    List<IClassDecorator> decorators = GetClassDecorators(property);
                    List<object> targets = GetTargets(property);

                    fieldHeight += decorators.Select(d => d.GetHeight(targets)).Sum();
                    fieldHeight += regularDecorators.Select(d => d.GetHeight(property)).Sum();
                }
            }

            return fieldHeight > 0 ? fieldHeight : 0;
        }

        private float GetInnerPropertyHeight(SerializedProperty property) {

            float innerHeight;
            if (IsPropertyArray(property)) {
                innerHeight =  GetListDrawer(property).GetHeight(property);
            }
            else {
                innerHeight = EditorGUI.GetPropertyHeight(property, new GUIContent(property.displayName));
            }
            
            return innerHeight > 0 ? innerHeight + 2 * FieldMargin : 0;
        }

        public void Draw(Rect rect, SerializedProperty property, bool showLabel = true) {
            Draw(rect, property, showLabel ? new GUIContent(property.displayName) : GUIContent.none);
        }

        public void Draw(Rect rect, SerializedProperty property, GUIContent label) {

            List<Decorator> regularDecorators = GetRegularDecorators(property);
            
            if (regularDecorators.All(d => d.ShowProperty(property))) {
                
                foreach (var decorator in regularDecorators) {
                    rect = decorator.OnBeforeGUI(rect, property);
                }

                if (MustBeShown(label)) {
                    Rect labelRect = new Rect(rect) {height = EditorGUIUtility.singleLineHeight};
                    property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);
                    rect.ToBottomOf(labelRect);
                    EditorGUI.indentLevel += 1;
                }
                else {
                    property.isExpanded = true;
                }

                if (property.isExpanded) {
                    
                    List<IClassDecorator> classDecorators = GetClassDecorators(property);
                    List<object> targets = GetTargets(property);
                    
                    foreach (var decorator in classDecorators) {
                        rect = decorator.OnInspectorGUIBefore(rect, targets);
                    }
                    
                    SerializedProperty nextProperty = property.Copy();
                    int startDepth = property.depth;
                    if (nextProperty.NextVisible(true)) {
                        do {
                            
                            Rect propertyRect = new Rect(rect) { height = GetInnerPropertyHeight(nextProperty) }; 
                            DrawInnerProperty(propertyRect, nextProperty);
                            rect.ToBottomOf(propertyRect);
                        } while (nextProperty.NextVisible(false) && startDepth < nextProperty.depth);
                    }
                    
                    foreach (var decorator in classDecorators) {
                        rect = decorator.OnInspectorGUIAfter(rect, targets);
                    }
                }
                
                foreach (var decorator in regularDecorators) {
                    rect = decorator.OnAfterGUI(rect, property);
                }

                if (MustBeShown(label)) {
                    EditorGUI.indentLevel -= 1;
                }
            }
        }

        private void DrawInnerProperty(Rect rect, SerializedProperty property, bool showLabel = true) {
            if (rect.height != 0) {
                rect.height -= FieldMargin * 2;
                rect.y += FieldMargin;
            }
            
            if (IsPropertyArray(property)) {
                DrawList(property, rect);
            }
            else {
                if (property.name == "m_Script")
                    GUI.enabled = false;
                EditorGUI.PropertyField(rect, property, showLabel ? new GUIContent(property.displayName) : GUIContent.none);
                if (property.name == "m_Script")
                    GUI.enabled = true;
            }
        }

        private static bool MustBeShown(GUIContent label) => label != null && label != GUIContent.none;

        private static bool IsPropertyArray(SerializedProperty property) 
            => property.isArray && property.propertyType == SerializedPropertyType.Generic;

        private bool HasCustomAttributes(SerializedProperty property) {
            EditorUtils.GetMemberInfo(property, out _, out var targetMemberInfo);

            List<Attribute> customAttributes = targetMemberInfo?.GetCustomAttributes().ToList();
            return targetMemberInfo != null && (customAttributes.Count > 2 || customAttributes.Count > 1 && customAttributes[0].GetType() != typeof(SerializeField));
        }

        private void DrawList(SerializedProperty property, Rect rect) {
            GetListDrawer(property).Draw(property, rect);
        }
        
        /// <summary>
        /// This class is used to draw a list in the unity editor, a bit differently than
        /// the regular editor list. Especially, this class uses the <see cref="ListPropertyDrawer"/>
        /// class, which takes in account the <see cref="BetterListAttribute">BetterList attribute</see>.
        /// </summary>
        private class ListDrawer {
            List<Decorator> Decorators;
            List<Decorator> DecoratorsReversed;
            private ListPropertyDrawer ActualDrawer;
            
            public ListDrawer(SerializedProperty property) {
                EditorUtils.GetMemberInfo(property, out _, out var targetMemberInfo);
                Decorators = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(targetMemberInfo);
                DecoratorsReversed = ((IEnumerable<Decorator>)Decorators).Reverse().ToList();
                
                ActualDrawer = new ListPropertyDrawer();
            }

            public float GetHeight(SerializedProperty property) {
                return Decorators.Select(d => d.GetHeight(property)).Sum()
                       + ActualDrawer.GetHeight(property, null);
            }
            
            public void Draw(SerializedProperty property, Rect rect) {
                rect.x += EditorGUI.indentLevel * 10;
                rect.width -= EditorGUI.indentLevel * 10;

                foreach (var decorator in Decorators) {
                    rect = decorator.OnBeforeGUI(rect, property);
                }

                rect = ActualDrawer.OnGUI(rect, property, null);
                
                foreach (var decorator in DecoratorsReversed) {
                    rect = decorator.OnAfterGUI(rect, property);
                }
            }
        }
    }
}
