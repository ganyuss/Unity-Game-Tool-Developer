using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class HelpBoxDecorator : DecoratorBase<HelpBoxAttribute> {
        public override float GetHeight(List<object> targets, string memberPath, SerializedProperty property) {
            return EditorUtils.HelpBox.GetHeight(CurrentAttribute.Text, CurrentAttribute.Type);
        }

        public override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            if (CurrentAttribute.Position == HelpBoxPosition.Before) {
                return DrawHelpBox(position);
            }

            return position;
        }

        public override Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            if (CurrentAttribute.Position == HelpBoxPosition.After) {
                return DrawHelpBox(position);
            }

            return position;
        }

        private Rect DrawHelpBox(Rect position) {
            return EditorUtils.HelpBox.Draw(position, CurrentAttribute.Text, CurrentAttribute.Type);
        }
    }
}