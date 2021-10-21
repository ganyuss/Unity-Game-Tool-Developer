using System;
using UnityEngine;

namespace EditorPlus {
    
    /// <summary>
    /// Attribute to have a callback method being called whenever the marked property
    /// get changed in the editor.
    /// </summary>
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets, AllowMultiple = true)]
    public class OnValueChangedAttribute : PropertyAttribute {
        
        /// <summary>
        /// The name of the method to call when the property gets modified.
        /// The method can either take no parameter, or a parameter of the target value.
        /// </summary>
        public string CallbackMemberName;

        public OnValueChangedAttribute(string callbackMemberName) {
            CallbackMemberName = callbackMemberName;
        }
    }
}

