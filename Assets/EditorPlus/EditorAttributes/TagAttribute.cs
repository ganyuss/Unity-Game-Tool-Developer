using System;
using UnityEngine;

namespace EditorPlus {
    
    /// <summary>
    /// Attribute to have a string field displayed as a dropdown, with all the existing
    /// tags in the project. Behave exactly like the tag dropdown at the top of a
    /// GameObject editor.
    /// </summary>
    [AttributeUsage(EditorPlusAttribute.AttributeDrawerTargets)]
    public class TagAttribute : PropertyAttribute
    {

    }
}