using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// This class can be used as a shortcut for Drawers that can only be applied to specific properties.
    /// implement <see cref="GetRealHeight">GetRealHeight</see> and <see cref="OnRealGUI">OnRealGUI</see>
    /// instead of <see cref="AttributeDrawerBase{Attr}.GetHeight">GetHeight</see> and
    /// <see cref="AttributeDrawerBase{Attr}.OnGUI">OnGUI</see>, and if <see cref="IsPropertyValid">IsPropertyValid</see>
    /// returns false, an error box will be shown instead of the property.
    /// </summary>
    /// <typeparam name="Attr"></typeparam>
    public abstract class PropertySpecificDrawerBase<Attr> : AttributeDrawerBase<Attr> where Attr : PropertyAttribute {
        
        public override float GetHeight(SerializedProperty property, GUIContent label) {
            if (IsPropertyValid(property, label)) {
                return GetRealHeight(property, label);
            }
            else {
                return EditorUtils.HelpBox.GetHeight(GetErrorText(property, label), HelpBoxType.Error);
            }
        }

        public override Rect OnGUI(Rect position, SerializedProperty property, GUIContent label) {

            if (IsPropertyValid(property, label)) {
                return OnRealGUI(position, property, label);
            }
            else {
                string helpBoxText = GetErrorText(property, label);
                return EditorUtils.HelpBox.Draw(position, helpBoxText, HelpBoxType.Error);
            }
        }

        protected abstract float GetRealHeight(SerializedProperty property, GUIContent label);
        protected abstract Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label);
        
        protected abstract bool IsPropertyValid(SerializedProperty property, GUIContent label);
        protected abstract string GetErrorText(SerializedProperty property, GUIContent label);
    }
}