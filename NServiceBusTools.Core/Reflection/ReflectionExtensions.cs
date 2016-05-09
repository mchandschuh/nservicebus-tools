using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using NServiceBusTools.Tree;

namespace NServiceBusTools.Reflection
{

    public static class ReflectionExtensions
    {
        public static TreeNode<MethodDefinition> GetCallTree(this MethodInfo methodInfo)
        {
            var assembly = methodInfo.DeclaringType.Assembly;
            var filepath = assembly.CodeBase.Substring("file:///".Length);
            var module = ModuleDefinition.ReadModule(filepath);

            var parameterFullNames = methodInfo.GetParameters().Select(x => x.ParameterType.FullName).ToList();
            var methodDefinition = (
                from type in module.GetTypes()
                where type.FullName == methodInfo.DeclaringType.FullName
                from method in type.GetMethods()
                where method.Name == methodInfo.Name
                where method.Parameters.Select(x => x.ParameterType.FullName).SequenceEqual(parameterFullNames)
                select method).SingleOrDefault();

            return methodDefinition.GetCallTree();
        }

        public static TreeNode<MethodDefinition> GetCallTree(this MethodDefinition method, int maxDepth = 2)
        {
            return method.GetCallTree(new MaxDepthTreeNavigator(maxDepth));
        }
        
        public static TreeNode<MethodDefinition> GetCallTree(this MethodDefinition method, ITreeNavigator navigator)
        {
            return TreeNode.Build(method, navigator, m =>
            {
                if (m.Body == null)
                {
                    return Enumerable.Empty<MethodDefinition>();
                }

                // get the methods called by this method
                return from instruction in m.Body.Instructions
                       where instruction.Operand is MethodReference
                       let methodReference = instruction.Operand as MethodReference
                       select methodReference.Resolve();
            });
        }

        public static bool IsEqualTo(this TypeReference typeReference, Type type)
        {
            return typeReference.FullName == type.FullName;
        }

        public static bool IsEqualTo(this TypeDefinition typeDefinition, Type type)
        {
            return typeDefinition.FullName == type.FullName;
        }

        public static bool IsEqualTo(this MethodDefinition methodDefinition, MethodInfo methodInfo)
        {
            if (!methodDefinition.DeclaringType.IsEqualTo(methodInfo.DeclaringType)) return false;
            if (methodDefinition.Name != methodInfo.Name) return false;

            var parameters = methodInfo.GetParameters();
            if (methodDefinition.Parameters.Count != parameters.Length) return false;

            for (int i = 0; i < parameters.Length; i++)
            {
                var cecilParm = methodDefinition.Parameters[i];
                var parm = parameters[i];
                if (!cecilParm.ParameterType.IsEqualTo(parm.ParameterType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
