using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NServiceBusTools.Tree
{
    /// <summary>
    /// Defines a node in a tree data structure. Each node is permitted to have
    /// exactly a single parent node and zero or more child nodes.
    /// </summary>
    public class TreeNode : IEnumerable<TreeNode>
    {
        private readonly UniqueList<TreeNode> _children;

        /// <summary>
        /// Constructs a new instance of a generic tree node with the specified children
        /// </summary>
        /// <typeparam name="T">The data type held at the tree noed</typeparam>
        /// <param name="data">The data at the tree root</param>
        /// <param name="children">The children of the new node</param>
        /// <returns>The root tree node</returns>
        public static TreeNode<T> Build<T>(T data, params TreeNode[] children)
        {
            return new TreeNode<T>(null, data, children);
        }

        /// <summary>
        /// Builds a new tree data structure returning the root node. The tree is recursively
        /// defined by selecting child nodes from each node. Be aware of infinite recursion, as
        /// this method will always step into children. Specify an instance of <see cref="ITreeNavigator{T}"/>
        /// to have more control over when to step into child nodes
        /// </summary>
        /// <typeparam name="T">The type of data held at each node</typeparam>
        /// <param name="root">The root of the tree structure</param>
        /// <param name="children">Function used to select child nodes from a node</param>
        /// <returns>The new tree structure built from the specified recursive definition function</returns>
        public static TreeNode<T> Build<T>(T root, Func<T, IEnumerable<T>> children)
        {
            return Build(root, NullTreeNavigator.Instance, children);
        }

        /// <summary>
        /// Builds a new tree data structure returning the root node. The tree is recursively
        /// defined by selecting child nodes from each node.
        /// </summary>
        /// <typeparam name="T">The type of data held at each node</typeparam>
        /// <param name="root">The root of the tree structure</param>
        /// <param name="navigator">Defines when to step into child nodes via the <see cref="ITreeNavigator{T}.NavigateDeeper"/> method</param>
        /// <param name="children">Function used to select child nodes from a node</param>
        /// <returns>The new tree structure built from the specified recursive definition function</returns>
        public static TreeNode<T> Build<T>(T root, ITreeNavigator navigator, Func<T, IEnumerable<T>> children)
        {
            return Build(null, root, navigator, children);
        }

        /// <summary>
        /// Builds a new tree data structure returning the root node of the produced sub-tree. The sub-tree
        /// is attached to specified parent node. The tree is recursively defined by selecting child nodes
        /// from each node.
        /// </summary>
        /// <typeparam name="T">The type of data held at each node</typeparam>
        /// <param name="parent">The parent node the produced sub-tree will be attached to</param>
        /// <param name="root">The root of the tree structure</param>
        /// <param name="navigator">Defines when to step into child nodes via the <see cref="ITreeNavigator{T}.NavigateDeeper"/> method</param>
        /// <param name="children">Function used to select child nodes from a node</param>
        /// <returns>The new tree structure built from the specified recursive definition function</returns>
        public static TreeNode<T> Build<T>(TreeNode parent, T root, ITreeNavigator navigator, Func<T, IEnumerable<T>> children)
        {
            var tree = new TreeNode<T>(parent, root);
            if (navigator.NavigateDeeper(tree))
            {
                var childNodes = children(root);
                if (childNodes != null)
                {
                    foreach (var child in childNodes)
                    {
                        var childTreeNode = Build(tree, child, navigator, children);
                        tree.Add(childTreeNode);
                    }
                }
            }
            return tree;
        }

        /// <summary>
        /// Gets the parent of this node, or null if this node is the root
        /// </summary>
        public TreeNode Parent
        {
            get; private set;
        }

        /// <summary>
        /// Gets all child nodes of this node
        /// </summary>
        public IEnumerable<TreeNode> Children
        {
            get { return _children.AsEnumerable(); }
        }

        /// <summary>
        /// Gets the depth of this node in the tree
        /// </summary>
        public int Depth
        {
            get
            {
                var depth = 0;
                var parent = Parent;
                while (parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }
                return depth;
            }
        }

        /// <summary>
        /// Gets the root node of the tree this node belongs to
        /// </summary>
        public TreeNode Root
        {
            get
            {
                var root = this;
                while (root.Parent != null) root = root.Parent;
                return root;
            }
        }

        /// <summary>
        /// Gets the maximum depth of nodes in the tree this node belongs to
        /// </summary>
        public int MaximumTreeDepth
        {
            get { return Root.Max(x => x.Depth); }
        }

        /// <summary>
        /// Initializes a new empty instance of the <see cref="TreeNode"/> class
        /// </summary>
        public TreeNode()
        {
            _children = new UniqueList<TreeNode>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNode"/> class
        /// </summary>
        /// <param name="parent">This node's parent node</param>
        /// <param name="children">This node's child nodes</param>
        public TreeNode(TreeNode parent, IEnumerable<TreeNode> children = null)
        {
            Parent = parent;
            _children = new UniqueList<TreeNode>();
            if (children != null)
            {
                foreach (var child in children)
                {
                    Add(child);
                }
            }
        }

        /// <summary>
        /// Attaches the specified node as this node's child. Assigns
        /// this node as the child's parent node
        /// </summary>
        /// <param name="child">The child node to be added</param>
        public void Add(TreeNode child)
        {
            if (_children.TryAdd(child))
            {
                child.Parent = this;
                _children.Add(child);
            }
        }

        /// <summary>
        /// Removes the specified child node
        /// </summary>
        /// <param name="child">The child node to be removes</param>
        /// <returns>True if the child node was removed, false if it was not a child</returns>
        public bool Remove(TreeNode child)
        {
            if (_children.Remove(child))
            {
                child.Parent = null;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes this node and recursively writes it child nodes to the specified text writer
        /// </summary>
        /// <param name="textWriter">The text writer instance to write to</param>
        public void WriteTo(TextWriter textWriter)
        {
            ToString(textWriter, 0);
        }

        public IEnumerator<TreeNode> GetEnumerator()
        {
            yield return this;
            foreach (var outter in Children)
            {
                foreach (var inner in outter)
                {
                    yield return inner;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return ValueToString();
        }

        /// <summary>
        /// Writes the value of this node to a string. Used by <see cref="WriteTo"/>
        /// and <see cref="ToString"/>
        /// </summary>
        /// <returns></returns>
        protected virtual string ValueToString()
        {
            return Depth.ToString();
        }

        private void ToString(TextWriter tw, int padding)
        {
            var pstr = new string('-', padding);
            tw.Write(pstr);
            tw.WriteLine(ValueToString());

            var newPadding = padding + 1;
            foreach (var child in Children)
            {
                child.ToString(tw, newPadding);
            }
        }
    }
}