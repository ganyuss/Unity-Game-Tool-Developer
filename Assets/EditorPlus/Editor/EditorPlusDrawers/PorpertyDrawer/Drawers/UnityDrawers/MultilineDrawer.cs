using System;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class MultilineDrawer : PropertySpecificDrawerBase<MultilineAttribute> {
        
        
        protected override float GetRealHeight(SerializedProperty property, GUIContent label) {
            return  EditorGUIUtility.singleLineHeight * CurrentAttribute.lines;
        }

        protected override Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect propertyRect = new Rect(position) {height = GetRealHeight(property, label)};

            Rect textAreaRect = label != null ? EditorGUI.PrefixLabel(propertyRect, label) : propertyRect;
            TextAreaDrawer.DrawTextArea(textAreaRect, property);
            
            position.ToBottomOf(propertyRect);
            return position;
        }

        protected override bool IsPropertyValid(SerializedProperty property, GUIContent label) {
            return string.Equals(property.type, "string", StringComparison.OrdinalIgnoreCase);
        }

        protected override string GetErrorText(SerializedProperty property, GUIContent label) {
            return $"{nameof(MultilineAttribute)} has been applied on the {property.displayName} field, " +
                   $"of type {property.type}. However it can only be used on strings.";
        }
    }
}
