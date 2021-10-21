using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace EditorPlus {
    
    /// <summary>
    /// Helper class to easily fetch types.
    /// </summary>
    public static class TypeUtils 
    {
        /// <summary>
        /// Returns all the types available.
        /// </summary>
        /// <returns>All the types.</returns>
        public static IEnumerable<Type> GetAllTypes() {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
        }
    
        /// <summary>
        /// This methods return a list of types with the given name. There can be
        /// maximum one type per assembly.
        /// </summary>
        /// <param name="typeName">The name of the types to look for.</param>
        /// <returns>The list of all the types with the given name.</returns>
        public static Type[] GetTypesFromName(string typeName) {
            List<Type> output = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Type type = assembly.GetType(typeName);
                if (type != null) output.Add(type);
            }

            return output.ToArray();
        }
        
        /// <summary>
        /// Returns all the types implementing the interface described by
        /// the interfaceType type.
        /// </summary>
        /// <param name="interfaceType">The type of the interface to search for.</param>
        /// <returns>All the types implementing the given interface.</returns>
        [NotNull]
        public static Type[] GetAllTypesImplementing(Type interfaceType) {

            if (!interfaceType.IsInterface) {
                return new Type[0];
            }
            
            List<Type> output = new List<Type>();
            
            foreach (Type type in GetAllTypes()) {
                if (type.GetInterfaces().Contains(interfaceType)) {
                    output.Add(type);
                }
            }

            return output.ToArray();
        }
        
        /// <summary>
        /// Returns all the types inheriting from the type described by
        /// the parentType type. This method does not take the generic
        /// parameters of classes. 
        /// </summary>
        /// <param name="parentType">The type to search the child classes of.</param>
        /// <returns>All the types inheriting from the given type.</returns>
        [NotNull]
        public static Type[] GetAllTypesInheritingFrom(Type parentType) {

            List<Type> output = new List<Type>();
            //parentType.gene
            
            foreach (Type type in GetAllTypes()) {
                Type currentType = type;
                while ((currentType = currentType.BaseType) != null) {
                    if (CorrespondTo(parentType, currentType)) {
                        output.Add(type);
                        break;
                    }
                }
            }

            return output.ToArray();
        }

        public static T CreateInstance<T>(Type type) {
            return (T) Activator.CreateInstance(type);
        }

        /// <summary>
        /// Compares two types, without taking in account the generic parameters.
        /// </summary>
        /// <example>
        /// <c>CorrespondTo(typeof(List&lt;&gt;), typeof(List&lt;string&gt;))</c> will return true.<br />
        /// <c>CorrespondTo(typeof(List&lt;&gt;), typeof(string))</c> will return false.
        /// </example>
        /// <param name="type1">The first type to test</param>
        /// <param name="type2">The second type to test the first type against</param>
        /// <returns>True if they describe the same type, whatever the generic parameters.</returns>
        private static bool CorrespondTo(Type type1, Type type2) {
            if (type1.IsEmptyGeneric())
                type1 = type1.GetGenericTypeDefinition();

            if (type2.IsEmptyGeneric())
                type2 = type2.GetGenericTypeDefinition();
            
            return type1 == type2;
        }
        
        private static bool IsEmptyGeneric(this Type type) {
            return type.IsGenericType && ! type.ContainsGenericParameters;
        }
    }
}