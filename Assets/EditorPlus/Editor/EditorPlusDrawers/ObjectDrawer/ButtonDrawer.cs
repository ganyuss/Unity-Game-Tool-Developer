using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// An <see cref="IClassDecorator" /> to draw buttons at the bottom of
    /// an editor, to trigger methods marked with the <see cref="ButtonAttribute">
    /// Button attribute</see>.
    /// </summary>
    public class ButtonDrawer : IClassDecorator {
        
        public string TargetPropertyPath { get; set; }
        
        private readonly float ButtonMargin = EditorGUIUtility.standardVerticalSpacing;
        
        private struct Button {
            public Action Action;
            public ButtonAttribute Attribute;
            public List<Decorator> Decorators;
            public string Name;
            public string MethodName;
            public List<object> Targets;

            public bool Equals(Button other) {
                return MethodName == other.MethodName
                       && Name == other.Name
                       && Attribute.Size == other.Attribute.Size
                       && Decorators.Select(d => d.GetType()).SequenceEqual(other.Decorators.Select(d => d.GetType()));
            }

            public void Merge(Button other) {
                Action += other.Action;
                Targets.AddRange(other.Targets);
            }
        }

        private readonly List<Button> ButtonsToDraw = new List<Button>();

        public void OnEnable(List<object> targets) {

            foreach (var target in targets) {
                List<MethodInfo> methods = GetAllButtonMethods(target);
                
                foreach (MethodInfo method in methods) {

                    var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();

                    if (buttonAttribute == null)
                        continue;

                    if (!IsSuitableForButton(method)) {
                        Debug.LogError(
                            $"Method \"{method.Name}\" got Button attribute, while not suitable for button calls");
                        continue;
                    }

                    ButtonsToDraw.Add(new Button {
                        Name = buttonAttribute.Name ?? method.Name,
                        Attribute = buttonAttribute,
                        Decorators = DecoratorAndDrawerDatabase.GetAllDecoratorsFor(method),
                        Action = (Action) method.CreateDelegate(typeof(Action), target),
                        MethodName = method.Name,
                        Targets = new List<object> { target }
                    });
                }
            }

            for (int i = 0; i < ButtonsToDraw.Count; i++) {
                Button currentButton = ButtonsToDraw[i];

                for (int j = ButtonsToDraw.Count - 1; j > i; j--) {
                    if (currentButton.Equals(ButtonsToDraw[j])) {
                        currentButton.Merge(ButtonsToDraw[j]);
                        ButtonsToDraw.RemoveAt(j);
                    }
                }
            }
        }

        public float GetHeight(List<object> targets) {
            float height = 0;
            foreach (Button button in ButtonsToDraw) {
                height += GetHeight(button) + ButtonMargin * 2;
            }
            
            return height;
        }

        private bool IsSuitableForButton(MethodInfo method) {
            return !method.IsAbstract && !method.IsConstructor && method.GetParameters().Length == 0;
        }

        private List<MethodInfo> GetAllButtonMethods(object target) {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                                       BindingFlags.NonPublic;
            return target.GetType().GetMethods(flags)
                .Where(method => method.GetCustomAttribute<ButtonAttribute>() != null)
                .ToList();
        }

        public Rect OnInspectorGUIBefore(Rect rect, List<object> targets) {
            return rect;
        }

        public Rect OnInspectorGUIAfter(Rect rect, List<object> targets) {

            // See EditorGUI.indent
            float indent = EditorGUI.indentLevel * 15f;
            
            // EditorGUI button do not take indent in account, we have to 
            // set it manually
            rect.x += indent;
            rect.width -= indent;

            foreach (var button in ButtonsToDraw) {
                rect = Draw(rect, button, targets);
            }
            
            
            rect.x -= indent;
            rect.width += indent;

            return rect;
        }

        private Rect Draw(Rect rect, Button button, IEnumerable<object> targets) {

            if (!button.Decorators.All(decorator => decorator.ShowProperty(button.Targets, button.MethodName)))
                return rect;
            
            Rect currentRect = new Rect(rect) { height = GetHeight(button) };
            rect.ToBottomOf(currentRect);
            currentRect.y += ButtonMargin;
            currentRect.height -= ButtonMargin;
            
            foreach (var decorator in button.Decorators) {
                currentRect = decorator.OnBeforeGUI(currentRect, button.Targets, button.MethodName);
            }
            

            Rect buttonRect = new Rect(currentRect) {height = GetButtonHeight(button.Attribute.Size)};
            if (GUI.Button(buttonRect, button.Name)) {
                button.Action.Invoke();
            }
            currentRect.ToBottomOf(buttonRect);
            
            List<Decorator> reversedDecorators = button.Decorators.ToList();
            reversedDecorators.Reverse();
            foreach (var decorator in reversedDecorators) {
                currentRect = decorator.OnAfterGUI(currentRect, button.Targets, button.MethodName);
            }

            return rect;
        }

        /// <summary>
        /// Returns the height of a given button, decorators included.
        /// </summary>
        /// <param name="button">A button.</param>
        /// <returns>The height of the given button.</returns>
        private float GetHeight(Button button) {
            if (!button.Decorators.All(decorator => decorator.ShowProperty(button.Targets, button.MethodName)))
                return 0;
            
            float height = button.Decorators.Select(d => d.GetHeight(button.Targets, button.MethodName)).Sum();
            height += GetButtonHeight(button.Attribute.Size);
            height += ButtonMargin * 2;
            
            return height;
        }

        /// <summary>
        /// Return the size of a button according to its <see cref="ButtonSize"/>.
        /// This method does not take in account the decorators around the button.
        /// </summary>
        /// <param name="size">The size attribute of the button.</param>
        /// <returns>The height of the button.</returns>
        private float GetButtonHeight(ButtonSize size) {
            switch (size) {
                case ButtonSize.Small:
                    return 20;
                default:
                case ButtonSize.Regular:
                    return 30;
                case ButtonSize.Large:
                    return 45;
                case ButtonSize.ExtraLarge:
                    return 60;
            }
        }
    }
}