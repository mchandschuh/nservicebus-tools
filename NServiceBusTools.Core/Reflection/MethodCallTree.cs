using System.Linq;
using Mono.Cecil;
using NServiceBusTools.Tree;

namespace NServiceBusTools.Reflection
{
    /// <summary>
    /// Provides extension methods that allow for the construction of method call trees
    /// </summary>
    public static class MethodCallTree
    {
        /// <summary>
        /// Gets the call tree under the specified method using a new instance of <see cref="MaxDepthTreeNavigator"/>
        /// with the specified <param name="maxDepth"></param>
        /// </summary>
        /// <param name="method">The method to produce a call tree for</param>
        /// <param name="maxDepth">The maximum depth of the tree</param>
        /// <returns>A new tree data structure representing the method call tree</returns>
        public static TreeNode<MethodDefinition> GetCallTree(this MethodDefinition method, int maxDepth = 5)
        {
            return method.GetCallTree(new MaxDepthTreeNavigator(maxDepth));
        }

        /// <summary>
        /// Gets the call tree under the specified method using the specified instance of <see cref="ITreeNavigator"/> to
        /// determine how deep into the tree we should map
        /// </summary>
        /// <param name="method">The method to produce a call tree for</param>
        /// <param name="navigator">Defines how deep into the tree we should navigate</param>
        /// <returns>A new tree data structure representing the method call tree</returns>
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
    }
}
