using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class HideInEditModeDecorator : DecoratorBase<HideInEditModeAttribute> {
        public override bool ShowProperty(List<object> targets, string memberPath, SerializedProperty property) => EditorApplication.isPlaying;
    }
    
    public class DisableInEditModeDecorator : DisableIfDecoratorBase<DisableInEditModeAttribute> {
        protected override bool Disable(List<object> targets, string memberPath) {
            return !EditorApplication.isPlaying;
        }
    }
}

