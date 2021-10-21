using UnityEngine;
using System;
using JetBrains.Annotations;
using UnityEditor;

namespace EditorPlus.Editor {

    /// <summary>
    /// This class is the base class of any drawer class.
    /// It is used as an abstraction layer to facilitate drawer manipulation.<br /><br />
    /// <b>DO NOT INHERIT FROM THIS CLASS</b>. If you want to create a custom drawer, inherit from
    /// <c>AttributeDrawerBase</c> instead.
    /// </summary>
    /// <seealso cref="AttributeDrawerBase{Attr}"/> 
    public abstract class Drawer {
        
        /// <summary>
        /// Returns the height of the property as drawn with this drawer, according
        /// to the associated <see cref="SerializedProperty"/> and <see cref="GUIContent">label</see>.
        /// </summary>
        /// <param name="property">The property to draw.</param>
        /// <param name="label">The associated label.</param>
        /// <returns>The height of the property as drawn in the editor buy the current drawer.</returns>
        public abstract float GetHeight(SerializedProperty property, [CanBeNull] GUIContent label);

        /// <summary>
        /// Draws the property at the top of the position <see cref="Rect"/>, and returns the part of
        /// position that has not be written on, for subsequent use.
        /// </summary>
        /// <param name="position">The Rect for the drawer to draw at the top of.</param>
        /// <param name="property">The property to draw.</param>
        /// <param name="label">The associated label.</param>
        /// <returns>The rect below the newly drawn property, inside the position Rect.</returns>
        public abstract Rect OnGUI(Rect position, SerializedProperty property, [CanBeNull] GUIContent label);

    }
    
    /// <summary>
    /// This class is the base class of the <see cref="AttributeDrawerBase{Attr}"/> class.
    /// It is used as an abstraction layer to facilitate its manipulation.<br /><br />
    /// <b>DO NOT INHERIT FROM THIS CLASS</b>. If you want to create a custom drawer, inherit from
    /// <c>AttributeDrawerBase&lt;Attr&gt;</c> instead.
    /// </summary>
    public abstract class AttributeDrawer : Drawer {
        public abstract Type AttributeType { get; }
        public abstract void SetAttribute(Attribute attribute);
    }
    
    /// <summary>
    /// Override this class to create a custom drawer associated to an attribute.
    /// Decorators are objects used to draw a field editor.
    /// </summary>
    /// <typeparam name="Attr">The attribute used to signal on what field to draw.</typeparam>
    /// <seealso cref="PropertySpecificDrawerBase{Attr}"/>
    public abstract class AttributeDrawerBase<Attr> : AttributeDrawer where Attr : PropertyAttribute {
        public override Type AttributeType => typeof(Attr);
        public Attr CurrentAttribute;
        
        
        public override void SetAttribute(Attribute attribute) {
            if (attribute.GetType() == AttributeType)
                CurrentAttribute = (Attr) attribute;
        }
    }
}
