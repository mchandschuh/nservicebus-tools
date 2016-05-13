using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceBusTools.Tree
{
    /// <summary>
    /// Defines a node in a tree data structure. Each node is permitted to have
    /// exactly a single parent node and zero or more child nodes. Each node also
    /// contains a piece of data of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of data held at each node</typeparam>
    public class TreeNode<T> : TreeNode
    {
        private readonly Func<T, string> _valueToString;

        /// <summary>
        /// Gets the type of data
        /// </summary>
        public Type DataType => typeof (T);

        /// <summary>
        /// Gets the data held at this node
        /// </summary>
        public T Data
        {
            get; set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode{T}"/> class
        /// </summary>
        /// <param name="parent">The parent node</param>
        /// <param name="data">The data held at this node</param>
        /// <param name="children">The child nodes</param>
        public TreeNode(TreeNode parent, T data, IEnumerable<TreeNode> children = null)
            : base(parent, children)
        {
            Data = data;
            _valueToString = x => x == null ? "[null]" : x.ToString();
        }

        /// <summary>
        /// Adds a new child node with the specified data
        /// </summary>
        /// <param name="data">The data to be held at the new child node</param>
        /// <returns>The newly added child node</returns>
        public TreeNode<T> Add(T data)
        {
            var child = new TreeNode<T>(null, data);
            Add(child);
            return child;
        }

        /// <summary>
        /// Removes the first child node with the specified data
        /// </summary>
        /// <param name="data">The data of the child to be removed</param>
        /// <returns>True if</returns>
        public bool Remove(T data)
        {
            var node = Children.OfType<TreeNode<T>>().FirstOrDefault(x => x.Data.Equals(data));
            if (node != null)
            {
                return Remove(node);
            }
            return false;
        }

        protected override string ValueToString()
        {
            return _valueToString(Data);
        }
    }
}
