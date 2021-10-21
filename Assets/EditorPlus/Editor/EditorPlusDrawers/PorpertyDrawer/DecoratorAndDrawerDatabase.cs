using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EditorPlus.Editor {
    
    /// <summary>
    /// This class is basically a factory for all the different attribute drawers and decorators
    /// created using EditorPlus.
    /// </summary>
    [InitializeOnLoad]
    public static class DecoratorAndDrawerDatabase {

        private static Dictionary<Type, Func<Decorator>> DecoratorFactoryDictionary;
        private static Dictionary<Type, Func<AttributeDrawer>> AttrbuteDrawerFactoryDictionary;

        /// <summary>
        /// Return all the different decorator attribute type registered.
        /// </summary>
        /// <returns>all the attributes used in a <see cref="DecoratorBase&lt;Attr&gt;"/>
        /// throughout the project.</returns>
        public static Type[] GetAllDecoratorAttributeTypes() {
            return DecoratorFactoryDictionary.Keys.ToArray();
        }
        
        /// <summary>
        /// Return all the different drawer attribute type registered.
        /// </summary>
        /// <returns>all the attributes used in a <see cref="AttributeDrawerBase&lt;Attr&gt;"/>
        /// throughout the project.</returns>
        public static Type[] GetAllDrawerAttributeTypes() {
            return AttrbuteDrawerFactoryDictionary.Keys.ToArray();
        }

        /// <summary>
        /// Returns true if the given type is the type of a decorator attribute. 
        /// </summary>
        /// <param name="attributeType">The type to test.</param>
        /// <returns>True if the given type is the type of a decorator attribute, otherwise false.</returns>
        public static bool IsDecoratorAttribute(Type attributeType) {
            return DecoratorFactoryDictionary.ContainsKey(attributeType);
        }
        
        /// <summary>
        /// Returns true if the given type is the type of a drawer attribute. 
        /// </summary>
        /// <param name="attributeType">The type to test.</param>
        /// <returns>True if the given type is the type of a drawer attribute, otherwise false.</returns>
        public static bool IsDrawerAttribute(Type attributeType) {
            return AttrbuteDrawerFactoryDictionary.ContainsKey(attributeType);
        }

        /// <summary>
        /// Returns an instance of the decorator class associated with the given decorator attribute type.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>An instance of the decorator type associated with the attribute type.</returns>
        public static Decorator GetDecoratorFor(Type attributeType) {
            return DecoratorFactoryDictionary[attributeType].Invoke();
        }

        /// <summary>
        /// Returns an instance of the drawer class associated with the given decorator attribute type.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <returns>An instance of the drawer type associated with the attribute type.</returns>
        public static AttributeDrawer GetDrawerFor(Type attributeType) {
            return AttrbuteDrawerFactoryDictionary[attributeType].Invoke();
        }

        /// <summary>
        /// If the given type is a decorator attribute, returns true and sets decorator
        /// to an instance of the decorator class associated with the given decorator attribute type.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <param name="decorator">Set to a decorator instance associated to the attribute type, if
        /// the attribute is a decorator attribute.</param>
        /// <returns>True if the given type is an decorator attribute, otherwise false.</returns>
        public static bool TryGetDecoratorFor(Type attributeType, out Decorator decorator) {
            bool ok = DecoratorFactoryDictionary.TryGetValue(attributeType, out var factory);
            decorator = factory?.Invoke();
            return ok;
        }
        
        /// <summary>
        /// If the given type is a drawer attribute, returns true and sets drawer
        /// to an instance of the drawer class associated with the given drawer attribute type.
        /// </summary>
        /// <param name="attributeType">The type of the attribute.</param>
        /// <param name="drawer">Set to a drawer instance associated to the attribute type, if
        /// the attribute is a drawer attribute.</param>
        /// <returns>True if the given type is an drawer attribute, otherwise false.</returns>
        public static bool TryGetDrawerFor(Type attributeType, out Drawer drawer) {
            bool ok = AttrbuteDrawerFactoryDictionary.TryGetValue(attributeType, out var factory);
            drawer = factory?.Invoke();
            return ok;
        }

        /// <summary>
        /// Returns a list of decorators associated with a given member.
        /// </summary>
        /// <param name="member">The member to get the decorator to decorate around.</param>
        /// <returns>The decorators associated with the members.</returns>
        public static List<Decorator> GetAllDecoratorsFor(MemberInfo member) {
            List<Decorator> result = new List<Decorator>();
            if (member is null)
                return result;
            
            foreach (var attribute in member.GetCustomAttributes()) {
                if (TryGetDecoratorFor(attribute.GetType(), out var decorator)) {
                    decorator.SetAttribute(attribute);
                    result.Add(decorator);
                }
            }

            result.Sort((d1, d2) => d1.Order - d2.Order);
            return result;
        }
        
        
        static DecoratorAndDrawerDatabase() {
            DecoratorFactoryDictionary = new Dictionary<Type, Func<Decorator>>();
            AttrbuteDrawerFactoryDictionary = new Dictionary<Type, Func<AttributeDrawer>>();
            
            Type[] DecoratorTypes = TypeUtils.GetAllTypesInheritingFrom(typeof(DecoratorBase<>));
            foreach (var decoratorType in DecoratorTypes) {
                if (IsInstantiationValid(decoratorType)) {
                    Decorator decorator = TypeUtils.CreateInstance<Decorator>(decoratorType);
                    DecoratorFactoryDictionary[decorator.AttributeType] = () => TypeUtils.CreateInstance<Decorator>(decoratorType);
                }
            }
            
            Type[] DrawerTypes = TypeUtils.GetAllTypesInheritingFrom(typeof(AttributeDrawerBase<>));
            foreach (var drawerType in DrawerTypes) {
                if (IsInstantiationValid(drawerType)) {
                    AttributeDrawer drawer = TypeUtils.CreateInstance<AttributeDrawer>(drawerType);
                    AttrbuteDrawerFactoryDictionary[drawer.AttributeType] = () => TypeUtils.CreateInstance<AttributeDrawer>(drawerType);
                }
            }
        }

        private static bool IsInstantiationValid(Type type) {
            return !type.IsGenericType && !type.IsAbstract;
        }
    }
}
