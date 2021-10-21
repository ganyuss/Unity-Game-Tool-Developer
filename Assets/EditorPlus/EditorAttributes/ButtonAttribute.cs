using System;
using JetBrains.Annotations;
using UnityEngine;

namespace EditorPlus {
    public enum ButtonSize {
        Small,
        Regular,
        Large,
        ExtraLarge
    }

    /// <summary>
    /// Attribute to create a button in the inspector for calling the method it is attached to.
    /// The method must have no arguments.
    /// </summary>
    /// <example><code>
    /// [Button]
    /// public void MyMethod()
    /// {
    ///     Debug.Log("Clicked!");
    /// }
    /// </code></example>
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class ButtonAttribute : PropertyAttribute {
        
        /// <summary>
        /// The label of the button. if left to null, will use the name
        /// of the associated method instead.
        /// </summary>
        public readonly string Name;

        public ButtonAttribute() { }

        public ButtonAttribute(string name) => Name = name;

        /// <summary>
        /// Indicates the size of the button.
        /// Defaults to <see cref="ButtonSize.Regular"/>.
        /// </summary>
        public ButtonSize Size = ButtonSize.Regular;
    }
}
