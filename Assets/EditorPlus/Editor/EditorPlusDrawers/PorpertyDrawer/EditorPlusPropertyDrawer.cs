using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// This class is responsible for drawing all the decorators and drawers.
    /// To update the list of attributes to draw with this class, use the
    /// <see cref="PropertyAttributeSetter" /> asset.
    /// </summary>
    /// <seealso cref="AttributeDrawerBase{Attr}"/>
    /// <seealso cref="DecoratorBase&lt;Attr&gt;"/>
    /// <seealso cref="DecoratorAndDrawerDatabase"/>
    public partial class EditorPlusPropertyDrawer : PropertyDrawer {

        private List<Decorator> _decoratorsToUse;
        private List<Decorator> _decoratorsToUseReversed;
        private Drawer _propertyDrawer;

        private List<Decorator> GetDecoratorsToUse(SerializedProperty property) {
            if (_decoratorsToUse != null)
                return _decoratorsToUse;

            // We do not want list decorators to also be used inside the list. 
            if (EditorUtils.IsForArrayElement(property.propertyPath)) {
                _decoratorsToUse = new List<Decorator>();
                return _decoratorsToUse;
            }
            
            EditorUtils.GetMemberInfo(property, out _, out var targetMemberInfo);
            _decoratorsToUse = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(targetMemberInfo);

            return _decoratorsToUse;
        }

        private List<Decorator> GetDecoratorsToUseReversed(SerializedProperty property) {
            if (_decoratorsToUseReversed != null)
                return _decoratorsToUseReversed;
            
            _decoratorsToUseReversed = ((IEnumerable<Decorator>)GetDecoratorsToUse(property)).Reverse().ToList();
            return _decoratorsToUseReversed;
        }
        
        private Drawer GetPropertyDrawer(SerializedProperty property) {
            if (_propertyDrawer != null)
                return _propertyDrawer;

            foreach (var drawerAttributeType in DecoratorAndDrawerDatabase.GetAllDrawerAttributeTypes()) {
                if (HasAttribute(property, drawerAttributeType, out var currentAttribute)) {
                    AttributeDrawer drawer = DecoratorAndDrawerDatabase.GetDrawerFor(drawerAttributeType);
                    drawer.SetAttribute(currentAttribute);
                    return drawer;
                }
            }

            _propertyDrawer = new DefaultDrawer();
            return _propertyDrawer;
        }
        

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            List<Decorator> decorators = GetDecoratorsToUse(property);

            if (!decorators.All(decorator => decorator.ShowProperty(property)))
                return 0;
            
            float height = 0;
            foreach (var decorator in decorators) {
                height += decorator.GetHeight(property);
            }

            float propertyHeight = GetPropertyDrawer(property).GetHeight(property, label);
            return height + propertyHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            List<Decorator> decorators = GetDecoratorsToUse(property);
            List<Decorator> decoratorsReversed = GetDecoratorsToUseReversed(property);
         
            if (!decorators.All(decorator => decorator.ShowProperty(property)))
                return;
            
            foreach (var decorator in decorators) {
                position = decorator.OnBeforeGUI(position, property);
            }

            if (decorators.All(decorator => decorator.ShowProperty(property))) {
                position = GetPropertyDrawer(property).OnGUI(position, property, label);
            }
            
            // We want the first decorator to be the last one called here
            foreach (var decorator in decoratorsReversed) {
                position = decorator.OnAfterGUI(position, property);
            }
        }
        
        
        private static bool HasAttribute(SerializedProperty property, Type attributeType, out Attribute attribute) {
            EditorUtils.GetMemberInfo(property, out _, out var targetMemberInfo);
            attribute = targetMemberInfo?.GetCustomAttribute(attributeType);
            return attribute != null;
        }
    }
    
    public class DefaultPropertyAttribute : PropertyAttribute { }

    /// <summary>
    /// This class is used as the default drawer, in a case where the <see cref="EditorPlusPropertyDrawer"/>
    /// class is used to draw a field on which there are only decorator attributes, and no drawer ones.
    /// <br /><br />
    /// This class uses <see cref="EditorGUI.PropertyField(Rect, SerializedProperty, GUIContent)"/> to draw
    /// the field. 
    /// </summary>
    public class DefaultDrawer : AttributeDrawerBase<DefaultPropertyAttribute> {
        
        public override float GetHeight(SerializedProperty property, GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label);
        }
        
        public override Rect OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            float height = GetHeight(property, label);
            Rect propertyRect = new Rect(position) {height = height};
            
            if (label != null && label.text == null)
                label.text = property.displayName;
            
            EditorGUI.PropertyField(propertyRect, property, label);
            
            position.ToBottomOf(propertyRect);
            return position;
        }
    }
}