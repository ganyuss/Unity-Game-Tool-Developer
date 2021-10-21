using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// Helper class with editor functions used in EditorPlus.
    /// </summary>
    public static class EditorUtils {

        public const string MultipleValueString = "—";
        
        /// <summary>
        /// The background color of the editor, depending on the editor skin selected.
        /// </summary>
        public static Color BackgroundColor {
            get {
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.22f, 0.22f, 0.22f);
                else {
                    return new Color(0.73f, 0.73f, 0.73f);
                }
            }
        }
        
        /// <summary>
        /// An alternative background color tu use in the editor. This color is used
        /// to differentiate elements in lists. 
        /// </summary>
        public static Color AccentBackgroundColor {
            get {
                if (EditorGUIUtility.isProSkin)
                    return new Color(0.25f, 0.25f, 0.25f);
                else {
                    return new Color(0.78f, 0.78f, 0.78f);
                }
            }
        }

        /// <summary>
        /// If otherRect has been created by copying rect and changing its height,
        /// Using this function will update rect to go right beneath otherRect.
        /// <br /><br />
        /// This is particularly useful in the decorator/drawer scheme used in EditorPlus,
        /// where decorators and drawers must return a new rect below what they
        /// drawn after drawing.
        /// </summary>
        /// <example><code>
        /// Rect controlRect = new Rect(rect) { height = GetControlHeight() };
        /// EditorGUI.DrawControl(controlRect);
        /// rect.ToBottomOf(controlRect);
        /// </code></example>
        /// <param name="rect">The rect to place at the bottom.</param>
        /// <param name="otherRect">The reference rect.</param>
        public static void ToBottomOf(this ref Rect rect, Rect otherRect) {
            float delta = otherRect.yMax - rect.y;
            rect.y += delta;
            rect.height -= delta;
        }

        /// <summary>
        /// Gives the member info associated to the given property, along with the
        /// parent instance the member is related to. It will search it starting from
        /// <c>property.serializedObject.targetObject</c>.
        /// </summary>
        /// <param name="property">The property we want to get the member from.</param>
        /// <param name="targetObject">The parent instance of the property.</param>
        /// <param name="targetMember">The member info related to the property.</param>
        /// <seealso cref="GetGeneralValue&lt;T&gt;"/>
        /// <seealso cref="SetGeneralValue"/>
        /// <seealso cref="SerializedProperty.hasMultipleDifferentValues">SerializedProperty.hasMultipleDifferentValues</seealso>
        public static void GetMemberInfo(SerializedProperty property, out object targetObject,
            out MemberInfo targetMember) 
            => GetMemberInfo(property.serializedObject.targetObject, property, out targetObject, out targetMember);
        
        /// <summary>
        /// Gives the member info associated to the given property, along with the
        /// parent instance the member is related to. The search will start from
        /// the given parent object instead of <c>property.serializedObject.targetObject</c>.
        /// <br /><br />
        /// This is especially useful when editing multiple objects.
        /// </summary>
        /// <param name="parentObject">The serialized object's target instance to start from.</param>
        /// <param name="property">The property we want to get the member from.</param>
        /// <param name="targetObject">The parent instance of the property.</param>
        /// <param name="targetMember">The member info related to the property.</param>
        /// <seealso cref="GetGeneralValue&lt;T&gt;"/>
        /// <seealso cref="SetGeneralValue"/>
        /// <seealso cref="SerializedProperty.hasMultipleDifferentValues">SerializedProperty.hasMultipleDifferentValues</seealso>
        public static void GetMemberInfo(object parentObject, SerializedProperty property, out object targetObject, out MemberInfo targetMember) {
            if (string.IsNullOrEmpty(property.propertyPath)) {
                targetObject = null;
                targetMember = null;
                return;
            }

            List<string> memberPath = property.propertyPath.Split('.').ToList();

            GetMemberInfo(parentObject, memberPath, out targetObject, out targetMember);
        }
        
        /// <summary>
        /// Gives the member info of the member name relativeMemberPath, sibling member
        /// of the member associated to the given property, along with the
        /// parent instance the member is related to. The search will start from
        /// the given parent object instead of <c>property.serializedObject.targetObject</c>.
        /// <br /><br />
        /// This is especially useful when editing multiple objects.
        /// </summary>
        /// <example>
        /// Let us take the class A as follows:
        /// <code>class A : ScriptableObject {public int b; public int c;}</code>
        /// If we have the SerializedProperty associated with the member "b",
        /// and we call this method with relativeMemberPath = "c", it will give
        /// the member info related to the "c" member.
        /// </example>
        /// <param name="parentObject">The serialized object's target instance to start from.</param>
        /// <param name="property">The property we want to get the member from.</param>
        /// <param name="relativeMemberPath">The name of the sibling member.</param>
        /// <param name="targetObject">The parent instance of the property.</param>
        /// <param name="targetMember">The member info related to the sibling of the property.</param>
        /// <seealso cref="GetGeneralValue&lt;T&gt;"/>
        /// <seealso cref="SetGeneralValue"/>
        /// <seealso cref="SerializedProperty.hasMultipleDifferentValues">SerializedProperty.hasMultipleDifferentValues</seealso>
        public static void GetMemberInfo(object parentObject, SerializedProperty property, string relativeMemberPath, out object targetObject, out MemberInfo targetMember) {
            if (string.IsNullOrEmpty(property.propertyPath)) {
                targetObject = null;
                targetMember = null;
                return;
            }

            List<string> memberPath = property.propertyPath.Split('.').ToList();
            memberPath[memberPath.Count - 1] = relativeMemberPath;

            GetMemberInfo(parentObject, memberPath, out targetObject, out targetMember);
        }

        /// <summary>
        /// Gives the member info of the member name relativeMemberPath, sibling member
        /// of the member associated to the given property, along with the
        /// parent instance the member is related to. It will search it starting from
        /// <c>property.serializedObject.targetObject</c>.
        /// </summary>
        /// <example>
        /// Let us take the class A as follows:
        /// <code>class A : ScriptableObject {public int b; public int c;}</code>
        /// If we have the SerializedProperty associated with the member "b",
        /// and we call this method with relativeMemberPath = "c", it will give
        /// the member info related to the "c" member.
        /// </example>
        /// <param name="property">The property we want to get the member from.</param>
        /// <param name="relativeMemberPath">The name of the sibling member.</param>
        /// <param name="targetObject">The parent instance of the property.</param>
        /// <param name="targetMember">The member info related to the sibling of the property.</param>
        /// <seealso cref="GetGeneralValue&lt;T&gt;"/>
        /// <seealso cref="SetGeneralValue"/>
        /// <seealso cref="SerializedProperty.hasMultipleDifferentValues">SerializedProperty.hasMultipleDifferentValues</seealso>
        public static void GetMemberInfo(
                SerializedProperty property, string relativeMemberPath, 
                out object targetObject, out MemberInfo targetMember) {
            object startingObject = property.serializedObject.targetObject;
            
            List<string> memberPath = property.propertyPath.Split('.').ToList();
            memberPath[memberPath.Count - 1] = relativeMemberPath;

            GetMemberInfo(startingObject, memberPath, out targetObject, out targetMember);
        }

        /// <summary>
        /// Gives the member info of the member at the end of the memberPath, along with the
        /// parent instance the member is related to. The member path must be a list of the different
        /// members to get from the startingObject to the target member. It muse also
        /// use the <see cref="SerializedProperty.propertyPath">SerializedProperty.propertyPath</see>
        /// syntax, especially for array elements.
        /// </summary>
        /// <param name="startingObject">The object to start the search from.</param>
        /// <param name="memberPath">The path to the target member.</param>
        /// <param name="targetObject">The parent instance of the member.</param>
        /// <param name="targetMember">The target member info.</param>
        /// <seealso cref="GetGeneralValue&lt;T&gt;"/>
        /// <seealso cref="SetGeneralValue"/>
        /// <seealso cref="SerializedProperty.hasMultipleDifferentValues">SerializedProperty.hasMultipleDifferentValues</seealso>
        public static void GetMemberInfo(object startingObject, List<string> memberPath,
                out object targetObject, out MemberInfo targetMember) {

            BindingFlags BindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
                                        | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.InvokeMethod;

            // Because we will change it, we copy it to prevent errors in the calling context
            memberPath = memberPath.ToList();
            targetMember = null;
            targetObject = startingObject;
            object nextObject = null;
            
            while (memberPath.Count > 0) {
                if (IsArrayPath(memberPath)) {

                    // If the path refers to an array element, we 
                    // return the array member
                    if (memberPath.Count == 2) {
                        memberPath.Clear();
                        continue;
                    }
                    
                    IList list = (IList) (nextObject ?? targetObject);
                    int valueIndex = GetArrayIndex(memberPath);

                    if (nextObject != null) {
                        targetObject = nextObject;
                    }
                    nextObject = list[valueIndex];
                    // In a property path, the path to the Nth element of an array is Array.data[N]
                    memberPath.RemoveAt(0);
                    memberPath.RemoveAt(0);
                }
                else {
                    targetMember = (nextObject ?? targetObject).GetType().GetMember(memberPath[0], BindingFlags).First();
                    memberPath.RemoveAt(0);

                    if (nextObject != null) {
                        targetObject = nextObject;
                    }
                    
                    if (memberPath.Count > 0) {
                        if (targetMember.MemberType == MemberTypes.Field) {
                            nextObject = ((FieldInfo) targetMember).GetValue(targetObject);
                        }
                        else {
                            throw new ArgumentException(
                                $"member {targetMember.Name} of class {targetObject.GetType().FullName} is " +
                                $"not a field, property or method");
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Return the value hold by the object's member described by the member info.
        /// Will work on member infos describing fields, properties or methods with no arguments. 
        /// </summary>
        /// <param name="obj">the object to get the value from.</param>
        /// <param name="member">The member of the object to get the value from.</param>
        /// <typeparam name="T">The expected type of the member's value.</typeparam>
        /// <returns>The value hold by the object's member</returns>
        /// <exception cref="ArgumentException">The member is not a field, a property nor a method.</exception>
        /// <exception cref="ArgumentException">The member is a method, but expect arguments.</exception>
        /// <exception cref="InvalidCastException">The member has the wrong type.</exception>
        public static T GetGeneralValue<T>(object obj, MemberInfo member) {
            if (member.MemberType == MemberTypes.Field) {
                return (T) ((FieldInfo) member).GetValue(obj);
            }
            if (member.MemberType == MemberTypes.Property) {
                return (T) ((PropertyInfo) member).GetValue(obj);
            }
            if (member.MemberType == MemberTypes.Method) {
                return (T) ((MethodInfo) member).Invoke(obj, new object[0]);
            }

            throw new ArgumentException("trying to get generic value of member that is not a property, field or method.");
        }
        
        /// <summary>
        /// Sets the value of the member of the parent object to the target value.
        /// Will work on member infos describing fields or properties. 
        /// </summary>
        /// <param name="parentObject">The object with the member to set.</param>
        /// <param name="member">The target member to set.</param>
        /// <param name="targetValue">The value to set the member value to.</param>
        /// <exception cref="ArgumentException">The member is not a field nor a property</exception>
        /// <exception cref="ArgumentException">The target value has the wrong type.</exception>
        public static void SetGeneralValue(object parentObject, MemberInfo member, object targetValue) {
            if (member.MemberType == MemberTypes.Field) {
                ((FieldInfo) member).SetValue(parentObject, targetValue);
                return;
            }
            if (member.MemberType == MemberTypes.Property) {
                ((PropertyInfo) member).SetValue(parentObject, targetValue);
                return;
            }

            throw new ArgumentException("trying to set generic value of member that is not a property or field.");
        }


        // In a property path, the path to the Nth element of an array is Array.data[N]
        private static readonly Regex ArrayDataRegex = new Regex(@"data\[([0-9]+)\]");
        
        private static bool IsArrayPath(List<string> memberPath) {
            return memberPath.Count >= 2 && memberPath[0] == "Array" && ArrayDataRegex.IsMatch(memberPath[1]);
        }
        
        private static int GetArrayIndex(List<string> memberPath) {
            // We get the first capture
            return int.Parse(ArrayDataRegex.Match(memberPath[1]).Groups[1].Captures[0].Value);
        }

        /// <summary>
        /// Returns true if the given member path is the path to
        /// an array element. It will return false if the target of the path
        /// is the value of a field inside an array element, and not directly
        /// an array element.
        /// </summary>
        /// <param name="memberPath">the member path to test.</param>
        /// <returns>true if the element described by the member path is an array element, otherwise false.</returns>
        public static bool IsForArrayElement(string memberPath) {
            return memberPath.EndsWith(/*data[N*/"]");
        }

        /// <summary>
        /// This method allows to compare a type to its serialized name, as seen
        /// in a <see cref="SerializedProperty"/>.
        /// </summary>
        /// <param name="type">The type to test</param>
        /// <param name="serializedTypeName">The name as it appears in the serialized property</param>
        /// <returns>true if the type name describes the given type, false otherwise</returns>
        /// <seealso cref="SerializedProperty.type">SerializedProperty.type</seealso>
        public static bool CompareType(Type type, string serializedTypeName) {
            Dictionary<string, Type> specialTypes = new Dictionary<string, Type> {
                {"int", typeof(Int32)},
                {"float", typeof(Single)},
                {"string", typeof(String)},
            };
            
            if (specialTypes.TryGetValue(serializedTypeName, out var specialType)) {
                return specialType == type;
            }

            return type.Name == serializedTypeName;
        }


        /// <summary>
        /// This subclass contains all the helper methods related to help boxes. 
        /// </summary>
        public static class HelpBox {
            private const int paddingHeight = 8;
            private const int marginHeight = 2;

            /// <summary>
            /// Returns the height required to draw a help box, depending on the text
            /// and the type of the box.
            /// </summary>
            /// <param name="boxText">The text that have to go inside the box.</param>
            /// <param name="type">The type of the box.</param>
            /// <returns>The vertical size of the box.</returns>
            public static float GetHeight(string boxText, HelpBoxType type) {
                // This stops icon shrinking if text content doesn't fill out the container enough.
                float minHeight = paddingHeight * 5;

                // Calculate the height of the HelpBox using the GUIStyle on the current skin and the inspector
                // window's currentViewWidth.
                var content = new GUIContent(boxText);
                if (type != HelpBoxType.None)
                    content.image = new Texture2D(55, 0);
                var style = GUI.skin.GetStyle("helpbox");

                var height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth);

                // We add tiny padding here to make sure the text is not overflowing the HelpBox from the top
                // and bottom.
                height += marginHeight * 2;

                // If the calculated HelpBox is less than our minimum height we use this to calculate the returned
                // height instead.
                if (type != HelpBoxType.None && height < minHeight)
                    height = minHeight;
                
                return height + EditorGUIUtility.standardVerticalSpacing*2;
            }
            
            private static Rect GetRect(Rect position, string boxText, HelpBoxType type) {
                return new Rect(position) {height = GetHeight(boxText, type)};
            }

            /// <summary>
            /// Draws a help box with the given content and type, at the top of the position
            /// rect.
            /// </summary>
            /// <param name="position">The rect to draw in.</param>
            /// <param name="boxText">The text to put in the box.</param>
            /// <param name="type">The type of the box.</param>
            /// <returns>The rect inside position below the help box, where the next drawers can draw.</returns>
            public static Rect Draw(Rect position, string boxText, HelpBoxType type) {
                Rect helpBoxRect = GetRect(position, boxText, type);
                position.ToBottomOf(helpBoxRect);

                helpBoxRect.y += EditorGUIUtility.standardVerticalSpacing;
                helpBoxRect.height -= EditorGUIUtility.standardVerticalSpacing * 2;
                EditorGUI.HelpBox(helpBoxRect, boxText, GetUnityMessageType(type));
                
                
                return position;
            }

            private static MessageType GetUnityMessageType(HelpBoxType helpBoxType) {
                switch (helpBoxType) {
                    case HelpBoxType.None: return MessageType.None;
                    case HelpBoxType.Info: return MessageType.Info;
                    case HelpBoxType.Warning: return MessageType.Warning;
                    case HelpBoxType.Error: return MessageType.Error;
                }
                
                return MessageType.None;
            }
        }
    }
}
