using System;
using UnityEngine;

namespace EditorPlus {
    /// <summary>
    /// Attribute to have space before and after the property.
    /// </summary>
    /// <seealso cref="SpaceAttribute"/>
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class CustomSpaceAttribute : PropertyAttribute {
        /// <summary>
        /// The height of the space before the property.
        /// </summary>
        public float SpaceBefore = 18f;
        /// <summary>
        /// The height of the space after the property.
        /// </summary>
        public float SpaceAfter = 0f;

        public CustomSpaceAttribute() { }

        public CustomSpaceAttribute(float spaceBefore) {
            SpaceBefore = spaceBefore;
        }
        
        public CustomSpaceAttribute(float spaceBefore, float spaceAfter) : this(spaceBefore) {
            SpaceAfter = spaceAfter;
        }
    }
}
