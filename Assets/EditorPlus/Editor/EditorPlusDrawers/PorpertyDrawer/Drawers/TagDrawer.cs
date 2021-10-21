using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public class TagDrawer : PropertySpecificDrawerBase<TagAttribute> {
        protected override float GetRealHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect propertyRect = new Rect(position) {height = GetRealHeight(property, label)};

            if (!InternalEditorUtility.tags.Contains(property.stringValue))
                property.stringValue = InternalEditorUtility.tags.FirstOrDefault() ?? "";
            
            property.stringValue = EditorGUI.TagField(propertyRect, label, property.stringValue);

            position.ToBottomOf(propertyRect);
            return position;
        }

        protected override bool IsPropertyValid(SerializedProperty property, GUIContent label) {
            return EditorUtils.CompareType(typeof(string), property.type);
        }

        protected override string GetErrorText(SerializedProperty property, GUIContent label) {
            return $"The Tag attribute can only be placed on string fields.";
        }
    }
}
