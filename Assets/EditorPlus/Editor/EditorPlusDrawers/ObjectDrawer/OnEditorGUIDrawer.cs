using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace EditorPlus.Editor {
    
    /// <summary>
    /// An <see cref="IClassDecorator" /> to call methods marked with the
    /// <see cref="OnEditorGUIAttribute">OnEditorGUI attribute</see>
    /// of an edited object.
    /// </summary>
    public class OnEditorGUIDrawer : IClassDecorator {
        
        public string TargetPropertyPath { get; set; }

        private List<Action> EditorCallbacks = new List<Action>();
        public void OnEnable(List<object> targets) {
            foreach (var target in targets) {
                EditorCallbacks.AddRange(GetEditorCallbacks(target));
            }
        }

        public float GetHeight(List<object> targets) => 0;

        private List<Action> GetEditorCallbacks(object obj) {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic;

            List<MethodInfo> methods = obj.GetType().GetMethods(flags)
                .Where(methodInfo => methodInfo.GetCustomAttribute<OnEditorGUIAttribute>() != null).ToList();

            for (int i = methods.Count - 1; i >= 0; i--) {
                if (!IsSuitableForEditorGUI(methods[i])) {
                    Debug.LogError($"Method {methods[i].Name} got the OnEditorGUI attribute, while not suitable for it.");
                    methods.RemoveAt(i);
                }
            }
            
            return methods.Select(methodInfo => (Action)methodInfo.CreateDelegate(typeof(Action), obj))
                .ToList();
        }

        private bool IsSuitableForEditorGUI(MethodInfo method) {
            return !method.IsConstructor && method.GetParameters().Length == 0;
        }

        public Rect OnInspectorGUIBefore(Rect rect, List<object> targets) {
            return rect;
        }

        public Rect OnInspectorGUIAfter(Rect rect, List<object> targets) {
            foreach (var editorCallback in EditorCallbacks) {
                editorCallback.Invoke();
            }

            return rect;
        }
    }
}
