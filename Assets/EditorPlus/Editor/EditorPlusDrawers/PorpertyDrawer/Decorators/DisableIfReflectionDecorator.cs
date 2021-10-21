using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace EditorPlus.Editor {
    public abstract class DisableIfReflectionDecoratorBase<Attr> : DisableIfDecoratorBase<Attr> where Attr : PropertyAttribute {
        
        private bool AttributeValueCorrect = true;
        
        protected abstract string MemberName { get; }
        protected abstract string AttributeName { get; }
        protected abstract bool PropertyDisabled(List<object> targets, string memberPath);
        
        protected sealed override bool Disable(List<object> targets, string memberPath) {
            try {
                return PropertyDisabled(targets, memberPath);
            }
            catch (Exception) {
                AttributeValueCorrect = false;
                return true;
            }
        }

        private string ErrorText => $"{AttributeName}: Member \"{MemberName}\" not found, or value not correct. " +
                                    $"It should be the name of an instance bool field, property or method " +
                                    $"with no parameters.";
        
        public sealed override float GetHeight(List<object> targets, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.GetHeight(targets, memberPath, property): EditorUtils.HelpBox.GetHeight(ErrorText, HelpBoxType.Error);
        }

        public sealed override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.OnBeforeGUI(position, targets, memberPath, property) : EditorUtils.HelpBox.Draw(position, ErrorText, HelpBoxType.Error);
        }
        
        public sealed override Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.OnAfterGUI(position, targets, memberPath, property) : position;
        }
    }

    public class DisableIfDecorator : DisableIfReflectionDecoratorBase<DisableIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        protected override string AttributeName => "DisableIf";
        
        protected override bool PropertyDisabled(List<object> targets, string memberPath) {
            List<string> memberPathList = memberPath.Split('.').ToList();
            memberPathList[memberPathList.Count - 1] = CurrentAttribute.MemberName;
            
            foreach (var target in targets) {
                EditorUtils.GetMemberInfo(target, memberPathList, out var targetObject, out var targetMember);
                if (Equals(EditorUtils.GetGeneralValue<object>(targetObject, targetMember),
                    CurrentAttribute.OptionalTargetValue))
                    return true;
            }

            return false;
        }
    }
    
    public class EnableIfDecorator : DisableIfReflectionDecoratorBase<EnableIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        protected override string AttributeName => "EnableIf";
        
        protected override bool PropertyDisabled(List<object> targets, string memberPath) {
            List<string> memberPathList = memberPath.Split('.').ToList();
            memberPathList[memberPathList.Count - 1] = CurrentAttribute.MemberName;
            
            foreach (var target in targets) {
                EditorUtils.GetMemberInfo(target, memberPathList, out var targetObject, out var targetMember);
                if (!Equals(EditorUtils.GetGeneralValue<object>(targetObject, targetMember),
                    CurrentAttribute.OptionalTargetValue))
                    return true;
            }

            return false;
        }
    }
}