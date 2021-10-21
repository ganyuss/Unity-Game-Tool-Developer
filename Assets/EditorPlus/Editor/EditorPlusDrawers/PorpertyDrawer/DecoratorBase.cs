using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {

    /// <summary>
    /// This class is the base class of the <see cref="EditorPlus.Editor.DecoratorBase&lt;Attr&gt;"/> class.
    /// It is used as a reference to decorators without taking in account the related attribute type.<br /><br />
    /// <b>DO NOT INHERIT FROM THIS CLASS</b>. If you want to create a custom decorator, inherit from
    /// <c>DecoratorBase</c> instead.
    /// </summary>
    public abstract class Decorator {
        public abstract Type AttributeType { get; }

        /// <summary>
        /// This enum allows the decorator to tell when it wants to be drawn related to other decorators.<br /><br />
        /// Note that decorators are drawn in a LIFO fashion: the first decorator called in the
        /// <see cref="OnBeforeGUI(Rect, List&lt;object&gt;, string,SerializedProperty)">OnBeforeGUI</see>
        /// phase will be the last in the
        /// <see cref="OnAfterGUI(Rect, List&lt;object&gt;, string,SerializedProperty)">OnAfterGUI</see> phase.
        /// </summary>
        public enum OrderValue : int {
            /// <summary>
            /// Use <see cref="VeryFirst"/> if the decorator must be drawn before the others. A
            /// decorator using this should not draw anything. Use it on decorators that change the GUI state,
            /// like decorators responsible for disabling fields, or decorators used for spacing.
            /// </summary>
            VeryFirst,
            /// <summary>
            /// Use <see cref="First"/> for decorator that must be the first ones to draw something in the editor,
            /// like spaces.
            /// </summary>
            First,
            /// <summary>
            /// <see cref="Regular"/> is the default <see cref="OrderValue"/> value.
            /// </summary>
            Regular,
            /// <summary>
            /// Use <see cref="Last"/> for decorator that must be drawn right over the property itself. For example,
            /// use it for attributes responsible for changing the property text colour, to not affect the other
            /// decorators.
            /// </summary>
            Last,
        }

        public virtual OrderValue Order => OrderValue.Regular;

        public bool ShowProperty(SerializedProperty property) =>
            ShowProperty(property.serializedObject.targetObjects.Select(o => (object)o).ToList(), property.propertyPath, property);

        public bool ShowProperty(List<object> targets, string memberPath) => ShowProperty(targets, memberPath, null);
        public virtual bool ShowProperty(List<object> targets, string memberPath, [CanBeNull] SerializedProperty property) => true;

        public float GetHeight(SerializedProperty property) => GetHeight(
            property.serializedObject.targetObjects.Select(o => (object) o).ToList(), property.propertyPath, property);
        public float GetHeight(List<object> targets, string memberPath) =>
            GetHeight(targets, memberPath, null);
        public virtual float GetHeight(List<object> targets, string memberPath, [CanBeNull] SerializedProperty property) {
            return 0;
        }

        public Rect OnBeforeGUI(Rect position, SerializedProperty property) => OnBeforeGUI(position,
            property.serializedObject.targetObjects.Select(o => (object) o).ToList(), property.propertyPath, property);
        public Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath) =>
            OnBeforeGUI(position, targets, memberPath, null);
        public virtual Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, [CanBeNull] SerializedProperty property) {
            return position;
        }

        public Rect OnAfterGUI(Rect position, SerializedProperty property) => OnAfterGUI(position,
            property.serializedObject.targetObjects.Select(o => (object) o).ToList(), property.propertyPath, property);
        public Rect OnAfterGUI(Rect position, List<object> targets, string memberPath) =>
            OnAfterGUI(position, targets, memberPath, null);
        public virtual Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, [CanBeNull] SerializedProperty property) {
            return position;
        }

        public abstract void SetAttribute(Attribute attr);
    }
    
    /// <summary>
    /// Override this class to create a custom decorator drawer. Decorators are objects used to
    /// draw in the editor <b>around</b> a field editor.
    /// </summary>
    /// <typeparam name="Attr">The attribute used to signal the decorator to draw around</typeparam>
    public abstract class DecoratorBase<Attr> : Decorator where Attr : PropertyAttribute {
        public sealed override Type AttributeType => typeof(Attr);
        public Attr CurrentAttribute;


        public sealed override void SetAttribute(Attribute attribute) {
            if (attribute.GetType() == AttributeType)
                CurrentAttribute = (Attr) attribute;
        }
    }
}