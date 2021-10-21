using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace EditorPlus.Editor {
    public class MinMaxDrawer : PropertySpecificDrawerBase<MinMaxSliderAttribute> {
        private const float rightFieldSize = 50f;
        private const float rightMargin = 5f;
        
        private SerializedProperty GetMinProperty(SerializedProperty property) {
            switch (property.type) {
                case nameof(MinMaxInt):
                    return property.FindPropertyRelative(nameof(MinMaxInt.Min));
                case nameof(MinMaxFloat):
                    return property.FindPropertyRelative(nameof(MinMaxFloat.Min));
                default:
                    return null;
            }
        }

        private SerializedProperty GetMaxProperty(SerializedProperty property) {
            switch (property.type) {
                case nameof(MinMaxInt):
                    return property.FindPropertyRelative(nameof(MinMaxInt.Max));
                case nameof(MinMaxFloat):
                    return property.FindPropertyRelative(nameof(MinMaxFloat.Max));
                default:
                    return null;
            }
        }

        protected override float GetRealHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label) {

            EditorGUI.BeginChangeCheck();
            
            SerializedProperty minProperty = GetMinProperty(property);
            SerializedProperty maxProperty = GetMaxProperty(property);

            float minValue;
            float maxValue;
            bool sliderDisabled = minProperty.hasMultipleDifferentValues
                                  || maxProperty.hasMultipleDifferentValues;
            bool isInteger = property.type == nameof(MinMaxInt);

            if (sliderDisabled) {
                minValue = float.NegativeInfinity;
                maxValue = float.PositiveInfinity;
            }
            else {
                if (isInteger) {
                    minValue = minProperty.intValue;
                    maxValue = maxProperty.intValue;
                }
                else {
                    minValue = minProperty.floatValue;
                    maxValue = maxProperty.floatValue;
                }
            }

            Rect sliderPosition = new Rect(position);
            sliderPosition.width -= rightFieldSize*2 + rightMargin*2;
            sliderPosition.height = EditorGUIUtility.singleLineHeight;
            
            EditorGUI.BeginDisabledGroup(sliderDisabled);
            EditorGUI.MinMaxSlider(sliderPosition, label, ref minValue, ref maxValue, CurrentAttribute.SliderMin, CurrentAttribute.SliderMax);
            EditorGUI.EndDisabledGroup();
            
            if (EditorGUI.EndChangeCheck()) {
                SetValue(property, minProperty, maxProperty, minValue, maxValue);
            }
            
            EditorGUI.BeginChangeCheck();

            Rect firstFieldPosition = new Rect(sliderPosition);
            firstFieldPosition.xMin += sliderPosition.width + rightMargin;
            firstFieldPosition.width = rightFieldSize;
            
            Rect secondFieldPosition = new Rect(firstFieldPosition);
            secondFieldPosition.xMin += rightFieldSize + rightMargin;
            secondFieldPosition.width = rightFieldSize;

            int previousIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            if (isInteger) {
                EditorGUI.DelayedIntField(firstFieldPosition, minProperty, GUIContent.none);
                EditorGUI.DelayedIntField(secondFieldPosition, maxProperty, GUIContent.none);
            }
            else {
                EditorGUI.DelayedFloatField(firstFieldPosition, minProperty, GUIContent.none);
                EditorGUI.DelayedFloatField(secondFieldPosition, maxProperty, GUIContent.none);
            }
            EditorGUI.indentLevel = previousIndent;

            if (EditorGUI.EndChangeCheck()) {

                float newMinValue;
                float newMaxValue;
                    
                if (property.type == nameof(MinMaxInt)) {
                    newMinValue = minProperty.intValue;
                    newMaxValue = maxProperty.intValue;
                }
                else {
                    newMinValue = minProperty.floatValue;
                    newMaxValue = maxProperty.floatValue;
                }
                
                newMinValue = Mathf.Clamp(newMinValue, CurrentAttribute.SliderMin, CurrentAttribute.SliderMax);
                newMaxValue = Mathf.Clamp(newMaxValue, CurrentAttribute.SliderMin, CurrentAttribute.SliderMax);
                
                if (newMinValue > newMaxValue) {
                    if (minValue != newMinValue)
                        newMaxValue = newMinValue;
                    else if (maxValue != newMaxValue)
                        newMinValue = newMaxValue;
                }
                
                SetValue(property, minProperty, maxProperty, newMinValue, newMaxValue);
            }

            position.ToBottomOf(sliderPosition);
            return position;
        }


        private void SetValue(
            SerializedProperty property,
            SerializedProperty minProperty, SerializedProperty maxProperty, 
            float min, float max) {
            if (property.type == nameof(MinMaxInt)) {
                minProperty.intValue = Mathf.RoundToInt(min);
                maxProperty.intValue = Mathf.RoundToInt(max);
            }
            else {
                minProperty.floatValue = min;
                maxProperty.floatValue = max;
            }
        }

        protected override bool IsPropertyValid(SerializedProperty property, GUIContent label) {
            return property.type == nameof(MinMaxInt) || property.type == nameof(MinMaxFloat);
        }

        protected override string GetErrorText(SerializedProperty property, GUIContent label) {
            return $"The MinMaxSlider is set on the {property.name} property of type {property.type}. " +
                $"MinMaxSlider only works on {nameof(MinMaxInt)} or {nameof(MinMaxFloat)} properties.";
        }
    }
}
