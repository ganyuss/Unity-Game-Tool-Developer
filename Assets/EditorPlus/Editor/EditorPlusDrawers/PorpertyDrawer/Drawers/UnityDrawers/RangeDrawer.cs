using System;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {

    public class RangeDrawer : PropertySpecificDrawerBase<RangeAttribute>
    {
        protected override float GetRealHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label) {
            Rect rangeRect = new Rect(position) { height = GetHeight(property, label) };

            if (EditorUtils.CompareType(typeof(int), property.type)) {
                EditorGUI.IntSlider(
                    rangeRect, property, 
                    Mathf.RoundToInt(CurrentAttribute.min), Mathf.RoundToInt(CurrentAttribute.max),
                    label);
            }
            else if (EditorUtils.CompareType(typeof(float), property.type)) {
                EditorGUI.Slider(
                    rangeRect, property, 
                    CurrentAttribute.min, CurrentAttribute.max,
                    label);
            }
            
            position.ToBottomOf(rangeRect);
            return position;
        }

        protected override bool IsPropertyValid(SerializedProperty property, GUIContent label) {
            return property.type == "int" || property.type == "float";
        }

        protected override string GetErrorText(SerializedProperty property, GUIContent label) {
            return $"Field {property.displayName} is of type {property.type}. However Range attribute" +
                   $"can only be used on int or float fields.";
        }
        
        
    }
}
