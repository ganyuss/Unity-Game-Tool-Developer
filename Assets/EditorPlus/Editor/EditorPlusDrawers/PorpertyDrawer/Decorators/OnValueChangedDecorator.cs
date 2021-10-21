using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace EditorPlus.Editor {
    
    public class OnValueChangedDecorator : DecoratorBase<OnValueChangedAttribute> {

        private string ErrorText => $"The OnValueChanged attribute has been given \"{CurrentAttribute.CallbackMemberName}\" " +
                                    $"as a callback method name, however the method does not exist, or does " +
                                    $"not fit the requirements.";

        /// <summary>
        /// This variable stores whether or not the value has been modified in the previous editor draw.
        /// We must do it this way to wait for the value change update in the model before calling the method(s),
        /// otherwise they will not be able to read the new value in the object.
        /// </summary>
        private bool ValueChangedPreviousFrame;

        public override OrderValue Order => OrderValue.Last;

        public override float GetHeight(List<object> targets, string memberPath, SerializedProperty property) {
            if (IsAttributeCorrect(targets, memberPath)) {
                return 0;
            }
            else {
                return EditorUtils.HelpBox.GetHeight(ErrorText, HelpBoxType.Error);
            }
        }

        public override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            
            if (ValueChangedPreviousFrame)
                CallCallbacks(targets, memberPath);
            
            if (IsAttributeCorrect(targets, memberPath)) {
                EditorGUI.BeginChangeCheck();
                return position;
            }
            else {
                return EditorUtils.HelpBox.Draw(position, ErrorText, HelpBoxType.Error);
            }
        }

        public override Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            if (IsAttributeCorrect(targets, memberPath)) {
                ValueChangedPreviousFrame = EditorGUI.EndChangeCheck();
            }
            
            return position;
        }


        private void CallCallbacks(List<object> targets, string memberPath) {
            List<string> memberPathList = GetMemberPathList(memberPath);
            
            foreach (var target in targets) {
                EditorUtils.GetMemberInfo(target, memberPathList, out var targetObject, out var targetMember);
                
                ((MethodInfo) targetMember).Invoke(targetObject, new object[]{});
            }
        }

        private bool IsAttributeCorrect(List<object> targets, string memberPath) {
            try {
                List<string> memberPathList = GetMemberPathList(memberPath);
                
                EditorUtils.GetMemberInfo(targets.First(), memberPathList, out _, out var targetMember);
                
                return targetMember.MemberType == MemberTypes.Method
                    && ((MethodInfo) targetMember).GetParameters().Length == 0;
            }
            catch (Exception) {
                return false;
            }
        }

        private List<string> GetMemberPathList(string memberPath) {
            List<string> memberPathList = memberPath.Split('.').ToList();
            memberPathList[memberPathList.Count - 1] = CurrentAttribute.CallbackMemberName;

            return memberPathList;
        }
    }
}
