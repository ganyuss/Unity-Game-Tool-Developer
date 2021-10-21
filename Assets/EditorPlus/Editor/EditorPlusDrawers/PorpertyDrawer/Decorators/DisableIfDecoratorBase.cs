using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public abstract class DisableIfDecoratorBase<Attr> : DecoratorBase<Attr> where Attr : PropertyAttribute {
        public override OrderValue Order => OrderValue.VeryFirst;

        protected abstract bool Disable(List<object> targets, string memberPath);
        
        private bool guiEnabled;

        public override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            
            if (Disable(targets, memberPath)) {
                guiEnabled = GUI.enabled;
                GUI.enabled = false;
            }
            
            return position;
        }

        public override Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            if (Disable(targets, memberPath))
                GUI.enabled = guiEnabled;
            
            return position;
        }
    }
}
