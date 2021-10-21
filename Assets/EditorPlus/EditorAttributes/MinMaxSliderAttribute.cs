using System;
using UnityEngine;


namespace EditorPlus {
    
    /// <summary>
    /// Attribute to have the field displayed as a min-max slider. Works like
    /// <see cref="RangeAttribute">Range</see>, but with two values on the slider.
    /// <br /><br />
    /// This attribute only works on <see cref="MinMaxInt"/> and <see cref="MinMaxFloat"/>
    /// fields.
    /// </summary>
    [AttributeUsage(EditorPlusAttribute.AttributeDrawerTargets)]
    public class MinMaxSliderAttribute : PropertyAttribute {

        /// <summary>
        /// The minimum value on the slider. 
        /// </summary>
        public float SliderMin;
        /// <summary>
        /// The maximum value on the slider. 
        /// </summary>
        public float SliderMax;

        public MinMaxSliderAttribute(float sliderMin, float sliderMax) {
            SliderMin = sliderMin;
            SliderMax = sliderMax;
        }
    }
}