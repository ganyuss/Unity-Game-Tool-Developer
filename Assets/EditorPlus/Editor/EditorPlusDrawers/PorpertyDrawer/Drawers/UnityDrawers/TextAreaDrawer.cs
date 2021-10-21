using System;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class TextAreaDrawer : PropertySpecificDrawerBase<TextAreaAttribute> {

        public Vector2 scrollPosition = Vector2.zero;
        
        private float GetTextAreaHeight(string text) {
            return Mathf.Clamp(
                GetTextHeight(text),
                CurrentAttribute.minLines * EditorStyles.textField.lineHeight,
                CurrentAttribute.maxLines * EditorStyles.textField.lineHeight);
        }
        
        protected override float GetRealHeight(SerializedProperty property, GUIContent label) {
            float labelHeight = label != null ? EditorGUIUtility.singleLineHeight : 0;
            return labelHeight + GetTextAreaHeight(property.stringValue);
        }

        protected override Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (label != null) {
                Rect labelRect = new Rect(position) {height = EditorGUIUtility.singleLineHeight};
                EditorGUI.LabelField(labelRect, label);
                
                position.ToBottomOf(labelRect);
            }

            /*
             FIXME: I could not make the scrollview work with textarea
            if (GetTextHeightInLines(textValue) > CurrentAttribute.maxLines) {
                Rect scrollViewRect = new Rect(position)
                    {height = CurrentAttribute.maxLines * EditorStyles.textField.lineHeight};

                scrollPosition = EditorUtils.ScrollView(scrollViewRect, GetTextHeight(textValue), scrollPosition, rect => {
                    textValue = EditorGUI.TextArea(rect, textValue);
                });
                
                position.ToBottomOf(scrollViewRect);
            }
            else {
            */
                Rect textAreaRect = new Rect(position) {height = GetTextAreaHeight(property.stringValue)};
                DrawTextArea(textAreaRect, property);
                
                position.ToBottomOf(textAreaRect);
            //}

            return position;
        }


        public static void DrawTextArea(Rect rect, SerializedProperty property) {
            string value;
            if (property.hasMultipleDifferentValues) {
                value = EditorUtils.MultipleValueString;
            }
            else {
                value = property.stringValue;
            }
            
            string newValue = EditorGUI.TextArea(rect, value);
            
            if (newValue != value && 
                (!property.hasMultipleDifferentValues || newValue != EditorUtils.MultipleValueString && !string.IsNullOrEmpty(newValue)))
                property.stringValue = newValue;
        }

        protected override bool IsPropertyValid(SerializedProperty property, GUIContent label) {
            return String.Equals(property.type, "string", StringComparison.CurrentCultureIgnoreCase);
        }

        protected override string GetErrorText(SerializedProperty property, GUIContent label) {
            return $"{property.displayName} is of type {property.type}, while {nameof(TextAreaAttribute)} can only" +
                   $"be applied to strings.";
        }

        private float GetTextHeight(string text) {
            var style = EditorStyles.textField;
            return style.CalcHeight(new GUIContent(text), EditorGUIUtility.currentViewWidth);
        }
        
        private float GetTextHeightInLines(string text) {
            return Mathf.CeilToInt(GetTextHeight(text) / EditorStyles.textField.lineHeight);
        }
    }
}
