using System;
using UnityEngine;

namespace EditorPlus {
    /// <summary>
    /// Attribute to have the field hidden When the editor is in play mode.
    /// </summary>
    /// <seealso cref="HideIfAttribute" />
    /// <seealso cref="HideInEditModeAttribute" />
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class HideInPlayModeAttribute : PropertyAttribute
    { }
    
    /// <summary>
    /// Attribute to have the field disabled When the editor is in play mode.
    /// </summary>
    /// <seealso cref="HideIfAttribute" />
    /// <seealso cref="DisableInEditModeAttribute" />
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class DisableInPlayModeAttribute : PropertyAttribute
    { }
}

