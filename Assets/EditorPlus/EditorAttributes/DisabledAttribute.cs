
using System;
using UnityEngine;

namespace EditorPlus {
    
    /// <summary>
    /// Attribute to have the member not interactable.
    /// <br /><br />
    /// Can be used on fields to have them read only in the editor,
    /// or to disable <see cref="ButtonAttribute">buttons</see>.
    /// </summary>
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class DisabledAttribute : PropertyAttribute { }
}
