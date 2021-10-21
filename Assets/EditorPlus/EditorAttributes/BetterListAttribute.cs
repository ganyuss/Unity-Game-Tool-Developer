using System;
using UnityEngine;

namespace EditorPlus {
    
    /// <summary>
    /// Attribute to customize list behaviours in the editor.
    /// <br /><br />
    /// It should only be placed on arrays and lists.
    /// </summary>
    [AttributeUsage(EditorPlusAttribute.AttributeDrawerTargets)]
    public class BetterListAttribute : PropertyAttribute {
        /// <summary>
        /// Should the list be always visible?
        /// </summary>
        public bool AlwaysExpanded = false;
        
        /// <summary>
        /// The name of the method to be called when the user clicks on the "+".
        /// <br /><br />
        /// This method must not have any parameter, and edit the list directly.
        /// </summary>
        public string AddMethod = null;
        /// <summary>
        /// The name of the method to be called when the user clicks on the "-".
        /// <br /><br />
        /// This method should have one parameter, either the index of the object to remove,
        /// or the object directly. The method must edit the list directly.
        /// </summary>
        public string RemoveMethod = null;

        /// <summary>
        /// Whether or not the "+" button should be displayed, at the bottom of the list.
        /// </summary>
        public bool ShowAdd = true;
        /// <summary>
        /// Whether or not the "-" button should be displayed, at the bottom of the list.
        /// </summary>
        public bool ShowRemove = true;
    }
}