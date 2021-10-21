using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EditorPlus.Editor;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    public class DropdownDrawer : PropertySpecificDrawerBase<DropdownAttribute> {
        private bool Initialized = false;

        private string ValueListMethod = null;
        private string[] StaticLabelList = null;
        private object[] StaticValueList = null;

        private void Initialize() {
            object attributeValue = CurrentAttribute.DropdownElements;

            if (attributeValue is string) {
                ValueListMethod = (string) attributeValue;
            }
            else {
                TryReadList(attributeValue, out StaticLabelList, out StaticValueList);
            }

            Initialized = true;
        }

        private void ReadList(object list, out string[] labelList, out object[] valueList) {
            if (!TryReadList(list, out labelList, out valueList)) {
                throw new ArgumentException("Provided list not readable.");
            }
        }
        
        private bool TryReadList(object list, out string[] labelList, out object[] valueList) {
            if (list is DropdownList dList) {
                labelList = dList.GetLabels();
                valueList = dList.GetValues();

                return true;
            }
            else if (list is IList iList) {
                List<object> values = new List<object>();
                foreach (var obj in iList)
                    values.Add(obj);

                valueList = values.ToArray();
                labelList = values.Select(x => x.ToString()).ToArray();
                return true;
            }

            labelList = null;
            valueList = null;
            return false;
        }

        private void GetDropDownValues(SerializedProperty property, out string[] labels, out object[] values) {
            if (!Initialized)
                Initialize();

            if (ValueListMethod != null) {
                List<string> labelList = null;
                List<object> valueList = null;
                
                foreach (var target in property.serializedObject.targetObjects) {
                    EditorUtils.GetMemberInfo(target, property, ValueListMethod, out var targetObject, out var targetMemberInfo);
                    object list = EditorUtils.GetGeneralValue<object>(targetObject, targetMemberInfo);
                    ReadList(list, out var tempLabels, out var tempValues);

                    if (labelList == null) {
                        labelList = tempLabels.ToList();
                        valueList = tempValues.ToList();
                    }
                    else {
                        for (int i = labelList.Count - 1; i >= 0; i--) {
                            if (!tempLabels.Contains(labelList[i])) {
                                labelList.RemoveAt(i);
                                valueList.RemoveAt(i);
                            }
                        }
                    }
                }

                labels = labelList?.ToArray();
                values = valueList?.ToArray();
            }
            else {
                labels = StaticLabelList;
                values = StaticValueList;
            }
        }

        private bool IsAttributeValueValid(SerializedProperty property) {
            if (!Initialized)
                Initialize();

            if (!Initialized) return false;

            if (string.IsNullOrEmpty(ValueListMethod))
                return true;
            
            try {
                EditorUtils.GetMemberInfo(property, ValueListMethod, out var targetObject, out var targetMemberInfo);
                object list = EditorUtils.GetGeneralValue<object>(targetObject, targetMemberInfo);
                if (!TryReadList(list, out _, out var valueList)) {
                    return false;
                }

                return valueList.Length > 0 && EditorUtils.CompareType(valueList.First().GetType(), property.type);
            }
            catch (Exception) {
                return false;
            }
        }
        
        
        protected override float GetRealHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight;
        }

        protected override Rect OnRealGUI(Rect position, SerializedProperty property, GUIContent label) {
            GetDropDownValues(property, out var labels, out var values);
            
            EditorUtils.GetMemberInfo(property, out var parentObject, out var targetMemberInfo);
            object currentObject = EditorUtils.GetGeneralValue<object>(parentObject, targetMemberInfo);

            int currentIndex = property.hasMultipleDifferentValues ? -1 : Array.IndexOf(values, currentObject);

            Rect propertyRect = new Rect(position) {height = GetRealHeight(property, label)};
            currentIndex = EditorGUI.Popup(propertyRect, label, currentIndex, labels.Select(s => new GUIContent(s)).ToArray());

            if (currentIndex != -1 && !Equals(values[currentIndex], currentObject)) {
                EditorUtils.SetGeneralValue(parentObject, targetMemberInfo, values[currentIndex]);
            }

            position.ToBottomOf(propertyRect);
            return position;
        }

        protected override bool IsPropertyValid(SerializedProperty property, GUIContent label) {
            return IsAttributeValueValid(property);
        }

        protected override string GetErrorText(SerializedProperty property, GUIContent label) {
            if (!Initialized) Initialize();
            
            if (!Initialized) {
                return "Dropdown: argument given to the attribute not valid.";
            }
            else {
                return $"Dropdown: member name \"{ValueListMethod}\" given to the attribute not valid. The member " +
                       $"must return either a list of {property.type}, or a DropdownList<{property.type}>. None of " +
                       $"them can be empty.";
            }
        }
    }
}
