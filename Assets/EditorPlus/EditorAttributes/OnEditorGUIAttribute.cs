using System;
using JetBrains.Annotations;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace EditorPlus {
    
    /// <summary>
    /// Attribute to have the specified method called on editor repaint.
    /// Editor repaint happen multiple times when the editor is interacted with,
    /// when the mouse hovers over it, or when the inspector target changes.
    /// <br /><br />
    /// The targeted method can be used to add content to the editor using <see cref="EditorGUILayout"/>,
    /// however it might not work correctly on classes used in inner fields. 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    [MeansImplicitUse]
    public class OnEditorGUIAttribute : PropertyAttribute {

    }
}
