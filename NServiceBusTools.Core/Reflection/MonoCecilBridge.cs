using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Rocks;

namespace NServiceBusTools.Reflection
{
    /// <summary>
    /// Provides methods for mapping between <see cref="System.Reflection"/> types and <see cref="Mono.Cecil"/> types
    /// </summary>
    public static class MonoCecilBridge
    {
        /// <summary>
        /// Converts the specified <see cref="Assembly"/> instance into the equivalent <see cref="AssemblyDefinition"/> instance.
        /// This method requires that the <see cref="Assembly"/>'s <see cref="Assembly.CodeBase"/> is a file on the local disk
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to be converted</param>
        /// <returns>The <see cref="AssemblyDefinition"/> matching the specified <see cref="Assembly"/></returns>
        public static AssemblyDefinition AsAssemblyDefinition(this Assembly assembly)
        {
            var filepath = assembly.CodeBase.Substring("file:///".Length);
            return AssemblyDefinition.ReadAssembly(filepath);
        }

        /// <summary>
        /// Converts the specified <see cref="MethodBase"/> instance into the equivalent <see cref="MethodDefinition"/> instance
        /// </summary>
        /// <param name="methodBase">The <see cref="MethodBase"/> to be converted</param>
        /// <returns>The <see cref="MethodDefinition"/> matching the specified <see cref="MethodBase"/></returns>
        public static MethodDefinition AsMethodDefinition(this MethodBase methodBase)
        {
            var assembly = methodBase.Module.Assembly;
            var module = assembly.AsAssemblyDefinition().MainModule;

            var methodDefinition = (
                from type in module.GetTypes()
                from method in type.GetMethods()
                where method.IsEquivalentTo(methodBase)
                select method
                ).SingleOrDefault();

            return methodDefinition;
        }

        /// <summary>
        /// Determines whether or not the specified <param name="typeDefinition"></param> and <param name="type"></param>
        /// are equivalent
        /// </summary>
        /// <param name="typeDefinition">The <see cref="TypeDefinition"/></param>
        /// <param name="type">The <see cref="Type"/></param>
        /// <returns>True if the specified types describe the same underlying type; false otherwise</returns>
        public static bool IsEquivalentTo(this TypeDefinition typeDefinition, Type type)
        {
            return typeDefinition.FullName == type.FullName;
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="TypeDefinition"/> and <see cref="Type"/> are equivalent
        /// </summary>
        /// <param name="type">The <see cref="Type"/></param>
        /// <param name="typeDefinition">The <see cref="TypeDefinition"/></param>
        /// <returns>True if the specified types describe the same underlying type; false otherwise</returns>
        public static bool IsEquivalentTo(this Type type, TypeDefinition typeDefinition)
        {
            return typeDefinition.IsEquivalentTo(type);
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="MethodDefinition"/> and <see cref="MethodBase"/>
        /// are equivalent
        /// </summary>
        /// <param name="methodDefinition">The <see cref="MethodDefinition"/></param>
        /// <param name="methodBase">The <see cref="MethodBase"/></param>
        /// <returns>True if the specified methods describe the same underlying method; false otherwise</returns>
        public static bool IsEquivalentTo(this MethodDefinition methodDefinition, MethodBase methodBase)
        {
            if (!methodDefinition.DeclaringType.IsEquivalentTo(methodBase.DeclaringType)) return false;
            if (methodDefinition.Name != methodBase.Name) return false;

            var parameters = methodBase.GetParameters();
            if (methodDefinition.Parameters.Count != parameters.Length) return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                var parmType = parameters[i].ParameterType;
                var cecilParmType = methodDefinition.Parameters[i].ParameterType.Resolve();
                if (!cecilParmType.IsEquivalentTo(parmType))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether or not the specified <see cref="MethodDefinition"/> and <see cref="MethodBase"/>
        /// are equivalent
        /// </summary>
        /// <param name="methodBase">The <see cref="MethodBase"/></param>
        /// <param name="methodDefinition">The <see cref="MethodDefinition"/></param>
        /// <returns>True if the specified methods describe the same underlying method; false otherwise</returns>
        public static bool IsEquivalentTo(this MethodBase methodBase, MethodDefinition methodDefinition)
        {
            return methodDefinition.IsEquivalentTo(methodBase);
        }
    }
}