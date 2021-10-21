using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class HideInPlayModeDecorator : DecoratorBase<HideInPlayModeAttribute> {
        public override bool ShowProperty(List<object> targets, string memberPath, SerializedProperty property) => !EditorApplication.isPlaying;
    }
    
    public class DisableInPlayModeDecorator : DisableIfDecoratorBase<DisableInPlayModeAttribute> {
        protected override bool Disable(List<object> targets, string memberPath) {
            return EditorApplication.isPlaying;
        }
    }
}

