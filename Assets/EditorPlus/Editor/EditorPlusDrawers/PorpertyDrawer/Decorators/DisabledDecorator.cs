using System.Collections.Generic;
using UnityEditor;

namespace EditorPlus.Editor {
    public class DisabledDecorator : DisableIfDecoratorBase<DisabledAttribute> {
        protected override bool Disable(List<object> targets, string memberPath) {
            return true;
        }
    }
}
