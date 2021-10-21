using System;
using UnityEngine;

namespace EditorPlus {
    /// <summary>
    /// Attribute to have the field hidden When the editor is <b>not</b> in play mode.
    /// </summary>
    /// <seealso cref="HideIfAttribute" />
    /// <seealso cref="HideInEditModeAttribute" />
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class HideInEditModeAttribute : PropertyAttribute
    { }
    
    /// <summary>
    /// Attribute to have the field disabled When the editor is <b>not</b> in play mode.
    /// </summary>
    /// <seealso cref="DisableIfAttribute" />
    /// <seealso cref="DisableInEditModeAttribute" />
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class DisableInEditModeAttribute : PropertyAttribute
    { }
}

