using System;
using UnityEngine;

namespace EditorPlus {
    
    /// <summary>
    /// Attribute to have the field disabled only if a member of the object returns
    /// a specific value.
    /// </summary>
    /// <seealso cref="DisabledAttribute"/>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="HideIfAttribute"/>
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets, AllowMultiple = true)]
    public class DisableIfAttribute : PropertyAttribute {
        /// <summary>
        /// The name of the member to test. Must be either the name of a field,
        /// property or method that takes no argument.
        /// <br /><br />
        /// If the member returns a value <see cref="object.Equals(object, object)">equal</see>
        /// to <see cref="OptionalTargetValue"/>, the field will be disabled.
        /// </summary>
        public readonly string MemberName;
        
        /// <summary>
        /// The value to expect from the member named <see cref="MemberName"/>.
        /// <br /><br />
        /// If the member returns a value <see cref="object.Equals(object, object)">equal</see>
        /// to this, the field will be disabled.
        /// </summary>
        public readonly object OptionalTargetValue;

        public DisableIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public DisableIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
    
    /// <summary>
    /// Attribute to have the field disabled only if a member of the object returns
    /// a value different from the one specified.
    /// </summary>
    /// <seealso cref="DisabledAttribute"/>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="ShowIfAttribute"/>
    [AttributeUsage(EditorPlusAttribute.DecoratorTargets, AllowMultiple = true)]
    public class EnableIfAttribute : PropertyAttribute {
        /// <summary>
        /// The name of the member to test. Must be either the name of a field,
        /// property or method that takes no argument.
        /// <br /><br />
        /// If the member returns a value <b>not</b> <see cref="object.Equals(object, object)">equal</see>
        /// to <see cref="OptionalTargetValue"/>, the field will be disabled.
        /// </summary>
        public string MemberName;
        
        /// <summary>
        /// The value to test the member named <see cref="MemberName"/> against.
        /// <br /><br />
        /// If the member returns a value <b>not</b> <see cref="object.Equals(object, object)">equal</see>
        /// to this, the field will be disabled.
        /// </summary>
        public object OptionalTargetValue;

        public EnableIfAttribute(string memberName) : this(memberName, true) {
        }
        
        public EnableIfAttribute(string memberName, object optionalTargetValue) {
            MemberName = memberName;
            OptionalTargetValue = optionalTargetValue;
        }
    }
}

