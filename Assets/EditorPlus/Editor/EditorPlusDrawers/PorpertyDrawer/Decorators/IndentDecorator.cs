using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace EditorPlus.Editor {

    public class IndentDecorator : DecoratorBase<IndentAttribute> {
        public override OrderValue Order => OrderValue.VeryFirst;

        private int IndentDelta;
        
        public override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            IndentDelta = CurrentAttribute.Value;
            if (EditorGUI.indentLevel + IndentDelta < 0) {
                IndentDelta = -EditorGUI.indentLevel;
            }
            
            EditorGUI.indentLevel += IndentDelta;
            
            return position;
        }

        public override Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            EditorGUI.indentLevel -= IndentDelta;
            
            return position;
        }
    }
}
