using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    public abstract class HideIfDecoratorBase<Attr> : DecoratorBase<Attr> where Attr : PropertyAttribute {
        
        public override OrderValue Order => OrderValue.Regular;
        private bool AttributeValueCorrect = true;
        
        protected abstract string MemberName { get; }
        protected abstract bool PropertyShown(List<object> targets, string memberPath);
        
        public sealed override bool ShowProperty(List<object> targets, string memberPath, SerializedProperty property) {
            try {
                return PropertyShown(targets, memberPath);
            }
            catch (Exception) {
                AttributeValueCorrect = false;
                return true;
            }
        }

        private string ErrorText => $"HideIf: Member \"{MemberName}\" not found, or value not correct. " +
                                    $"It should be the name of an instance bool field, property or method " +
                                    $"with no parameters.";
        
        public override float GetHeight(List<object> targets, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.GetHeight(targets, memberPath, property): EditorUtils.HelpBox.GetHeight(ErrorText, HelpBoxType.Error);
        }

        public override Rect OnBeforeGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.OnBeforeGUI(position, targets, memberPath, property) : EditorUtils.HelpBox.Draw(position, ErrorText, HelpBoxType.Error);
        }
        
        public override Rect OnAfterGUI(Rect position, List<object> targets, string memberPath, SerializedProperty property) {
            return AttributeValueCorrect ? base.OnAfterGUI(position, targets, memberPath, property) : position;
        }
    }

    public class HideIfDecorator : HideIfDecoratorBase<HideIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        protected override bool PropertyShown(List<object> targets, string memberPath) {
            List<string> memberPathList = memberPath.Split('.').ToList();
            memberPathList[memberPathList.Count - 1] = CurrentAttribute.MemberName;

            foreach (var target in targets) {
                EditorUtils.GetMemberInfo(target, memberPathList, out var targetObject, out var targetMember);
                if (Equals(EditorUtils.GetGeneralValue<object>(targetObject, targetMember),
                    CurrentAttribute.OptionalTargetValue))
                    return false;
            }

            return true;
        }
    }
    
    public class ShowIfDecorator : HideIfDecoratorBase<ShowIfAttribute> {
        protected override string MemberName => CurrentAttribute.MemberName;
        protected override bool PropertyShown(List<object> targets, string memberPath) {
            List<string> memberPathList = memberPath.Split('.').ToList();
            memberPathList[memberPathList.Count - 1] = CurrentAttribute.MemberName;

            foreach (var target in targets) {
                EditorUtils.GetMemberInfo(target, memberPathList, out var targetObject, out var targetMember);
                if (!Equals(EditorUtils.GetGeneralValue<object>(targetObject, targetMember),
                    CurrentAttribute.OptionalTargetValue))
                    return false;
            }

            return true;
        }
    }
}