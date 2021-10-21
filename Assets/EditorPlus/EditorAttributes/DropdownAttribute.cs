using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EditorPlus {
    /// <summary>
    /// Replace the regular editor field with a dropdown with the specified values.
    /// <br />
    /// A method name can also be given.
    /// </summary>
    /// <example><code>
    /// [Dropdown(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 })]
    /// public int Digit;
    /// </code></example>
    /// <seealso cref="DropdownElements"/>
    [AttributeUsage(EditorPlusAttribute.AttributeDrawerTargets)]
    public class DropdownAttribute : PropertyAttribute {
        /// <summary>
        /// The value used to specify what to show in the dropdown. It can be either:
        /// <ul>
        /// <li>
        /// an array of objects. They will be displayed in the dropdown using <see cref="object.ToString()"/>,
        /// and their values will be to the field when selected. 
        /// </li>
        /// <li>
        /// The name of a field, property or method returning an array of objects.
        /// They will be displayed in the dropdown using <see cref="object.ToString()"/>,
        /// and their values will be to the field when selected. The method must take
        /// no argument.
        /// </li>
        /// <li>
        /// The name of a field, property or method returning an instance of a <see cref="DropdownList&lt;Element&gt;"/>.
        /// They will be displayed in the dropdown using the provided labels,
        /// and their values will be to the field when selected. The method must take
        /// no argument.
        /// </li>
        /// </ul>
        /// </summary>
        /// <example><code>
        /// [Dropdown(nameof(dropdownInt))]
        /// public int DropdownTest;
        /// private DropdownList&lt;int&gt; dropdownInt()
        ///             => new DropdownList&lt;int&gt; {["one"] = 1, ["two"] = 2};
        /// </code></example>
        public object DropdownElements;

        public DropdownAttribute(object elements) {
            DropdownElements = elements;
        }
    }

    /// <summary>
    /// See <see cref="DropdownList&lt;Element&gt;" />.
    /// </summary>
    public abstract class DropdownList {

        public abstract string[] GetLabels();
        public abstract object[] GetValues();
    }

    /// <summary>
    /// Describes a dropdown to show using the <see cref="DropdownAttribute"/>.
    /// </summary>
    /// <typeparam name="Element">The type of the dropdown's element. Must be
    /// a type of value that can be set to the field marked with the
    /// <see cref="DropdownAttribute"/>.</typeparam>
    public class DropdownList<Element> : DropdownList {
        private List<string> Labels = new List<string>();
        private List<Element> Values = new List<Element>();
        
        private void AddEntry(string label, Element element) {
            if (Labels.Contains(label))
                throw new ArgumentException($"Label {label} already in the DropdownList");
            Labels.Add(label);
            Values.Add(element);
        }

        /// <summary>
        /// Use this to add/edit/read values of the dropdown list.
        /// </summary>
        /// <param name="label">The name to display the given value with in the dropdown.</param>
        public Element this[string label] {
            get => Values[Labels.IndexOf(label)];
            set {
                if (Labels.Contains(label)) {
                    Values[Labels.IndexOf(label)] = value;
                }
                else {
                    AddEntry(label, value);
                }
            }
        }

        /// <summary>
        /// This method can be used to get all the labels of the DropdownList.
        /// It is mainly used by the drawer to draw the dropdown.
        /// </summary>
        /// <returns>The list of the labels in the DropdownList</returns>
        public override string[] GetLabels() {
            return Labels.ToArray();
        }

        /// <summary>
        /// This method can be used to get all the values of the DropdownList, without the type.
        /// It is mainly used by the drawer to draw the dropdown.
        /// </summary>
        /// <returns>The list of the values in the DropdownList</returns>
        public override object[] GetValues() {
            return Values.Select(e => (object)e).ToArray();
        }
    }
}
