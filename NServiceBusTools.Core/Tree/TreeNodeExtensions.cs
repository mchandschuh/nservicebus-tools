using System;
using System.Collections.Generic;
using System.Linq;

namespace NServiceBusTools.Tree
{
    public static class TreeNodeExtensions
    {
        public static IEnumerable<TreeNode> AsEnumerable(this TreeNode node)
        {
            yield return node;
            foreach (var outter in node.Children())
            {
                foreach (var inner in outter.AsEnumerable())
                {
                    yield return inner;
                }
            }
        }

        public static IEnumerable<TreeNode<T>> AsEnumerable<T>(this TreeNode<T> node)
        {
            yield return node;
            foreach (var outter in node.Children())
            {
                foreach (var inner in outter.AsEnumerable())
                {
                    yield return inner;
                }
            }
        }

        public static IEnumerable<TreeNode> Children(this TreeNode node)
        {
            return node.GetChildren();
        }

        public static IEnumerable<TreeNode<T>> Children<T>(this TreeNode<T> node)
        {
            return node.GetChildren().OfType<TreeNode<T>>();
        }

        public static TreeNode<TResult> Select<T, TResult>(this TreeNode<T> tree, Func<TreeNode<T>, TreeNode<TResult>> selector)
        {
            return (TreeNode<TResult>) new SelectTreeVisitor(n => selector((TreeNode<T>) n)).Visit(tree);
        }

        public static TreeNode<T> Where<T>(this TreeNode<T> tree, Func<TreeNode<T>, bool> predicate)
        {
            return (TreeNode<T>) new WhereTreeVisitor(n => predicate((TreeNode<T>) n)).Visit(tree);
        }
    }

    public class WhereTreeVisitor : TreeVisitor
    {
        private readonly Func<TreeNode, bool> predicate;

        public WhereTreeVisitor(Func<TreeNode, bool> predicate)
        {
            this.predicate = predicate;
        }

        public override TreeNode Visit(TreeNode node)
        {
            if (predicate(node))
            {
                return base.Visit(node);
            }
            return null;
        }
    }

    public class TreeVisitor
    {
        public virtual TreeNode Visit(TreeNode node)
        {
            return VisitChildren(node);
        }

        public virtual TreeNode VisitChildren(TreeNode node)
        {
            foreach (var child in node.Children())
            {
                var result = Visit(child);
                if (result != child)
                {
                    node.Remove(child);
                }
                if (result != null)
                {
                    node.Add(result);
                }
            }
            return node;
        }
    }

    public class SelectTreeVisitor : TreeVisitor
    {
        private readonly Func<TreeNode, TreeNode> selector;

        public SelectTreeVisitor(Func<TreeNode, TreeNode> selector)
        {
            this.selector = selector;
        }
        public override TreeNode Visit(TreeNode node)
        {
            return VisitChildren(selector(node));
        }
    }
}
