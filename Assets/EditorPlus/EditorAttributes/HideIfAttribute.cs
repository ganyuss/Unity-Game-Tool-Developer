using System;
using UnityEngine;

namespace EditorPlus {
    /// <summary>
    /// Attribute to have the field hidden only if a member of the object returns
    /// a specific value.
    /// </summary>
    /// <seealso cref="ShowIfAttribute" />
    /// <seealso cref="HideInInspector" />
    /// <seealso cref="DisableIfAttribute" />
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute {
        /// <summary>
        /// The name of the member to test. Must be either the name of a field,
        /// property or method that takes no argument.
        /// <br /><br />
        /// If the member returns a value <see cref="object.Equals(object, object)">equal</see>
        /// to <see cref="OptionalTargetValue"/>, the field will be hidden.
        /// </summary>
        public string MemberName;
        
        /// <summary>
        /// The value to expect from the member named <see cref="MemberName"/>.
        /// <br /><br />
        /// If the member returns a value <see cref="object.Equals(object, object)">equal</see>
        /// to this, the field will be hidden.
        /// </summary>
        public object OptionalTargetValue;

        public HideIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public HideIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
    
    /// <summary>
    /// Attribute to have the field hidden only if a member of the object returns
    /// a value different from the one specified.
    /// </summary>
    /// <seealso cref="ShowIfAttribute" />
    /// <seealso cref="HideInInspector" />
    /// <seealso cref="EnableIfAttribute" />
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute {
        /// <summary>
        /// The name of the member to test. Must be either the name of a field,
        /// property or method that takes no argument.
        /// <br /><br />
        /// If the member returns a value <b>not</b> <see cref="object.Equals(object, object)">equal</see>
        /// to <see cref="OptionalTargetValue"/>, the field will be hidden.
        /// </summary>
        public string MemberName;
        
        /// <summary>
        /// The value to test the member named <see cref="MemberName"/> against.
        /// <br /><br />
        /// If the member returns a value <b>not</b> <see cref="object.Equals(object, object)">equal</see>
        /// to this, the field will be disabled.
        /// </summary>
        public object OptionalTargetValue;

        public ShowIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public ShowIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
}
