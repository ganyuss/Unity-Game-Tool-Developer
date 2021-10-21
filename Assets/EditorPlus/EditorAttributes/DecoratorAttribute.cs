using System;
using UnityEngine;

namespace EditorPlus {
    public static class EditorPlusAttribute {
        /// <summary>
        /// The attribute target to set for an all purpose Decorator willing to work
        /// with the EditorPlus editor.
        /// </summary>
        public const AttributeTargets DecoratorTargets = AttributeTargets.Field | AttributeTargets.Method;
        /// <summary>
        /// The attribute target to set for an attribute Drawer willing to work
        /// with the EditorPlus editor.
        /// </summary>
        public const AttributeTargets AttributeDrawerTargets = AttributeTargets.Field;
    }
}
