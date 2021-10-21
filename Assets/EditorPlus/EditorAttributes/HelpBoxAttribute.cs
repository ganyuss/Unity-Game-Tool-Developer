using System;
using UnityEngine;

namespace EditorPlus {
    
    public enum HelpBoxType {
        None,
        Info,
        Warning,
        Error
    }
    
    /// <summary>
    /// The position of a help box, relative to the property drawer. 
    /// </summary>
    public enum HelpBoxPosition {
        Before,
        After
    }
    
    /// <summary>
    /// Attribute to add a help box around a field editor. 
    /// </summary>
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets)]
    public class HelpBoxAttribute : PropertyAttribute {
        
        /// <summary>
        /// The content of the help box.
        /// </summary>
        public string Text;
        /// <summary>
        /// The type of the icon to display on the left hand side of the help box.
        /// </summary>
        public HelpBoxType Type = HelpBoxType.Info;
        /// <summary>
        /// The position of the help box, relative to the property drawer. 
        /// </summary>
        public HelpBoxPosition Position = HelpBoxPosition.Before;
        
        public HelpBoxAttribute(string text) => Text = text;
        
        public HelpBoxAttribute(string text, HelpBoxType type) : this(text) {
            Type = type;
        }
    }
}
