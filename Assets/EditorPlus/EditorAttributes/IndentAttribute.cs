using System;
using UnityEngine;

namespace EditorPlus {
    
    /// <summary>
    /// Attribute to have the target field being indented to the left or the right.
    /// </summary>
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class IndentAttribute : PropertyAttribute {

        /// <summary>
        /// Use this value as an indent value to force the field to align to the far left of
        /// the editor, by removing all indentation.
        /// </summary>
        public const int RemoveIntent = int.MinValue;
        
        /// <summary>
        /// The value of indentation to add to the field, relative to its supposed value.
        /// This value can be either positive or negative, but the resulting indent cannot go below 0.
        /// </summary>
        public int Value;

        public IndentAttribute(int value) {
            Value = value;
        }
    }
}
