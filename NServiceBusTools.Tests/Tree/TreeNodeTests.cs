using System.Linq;
using NUnit.Framework;
using System.Collections.Generic;

namespace NServiceBusTools.Tree
{
    public static class TreeNodeTests
    {
        [TestFixture]
        public class WhenDefaultTreeNodeInitialized
        {
            private TreeNode _tree;

            [SetUp]
            public void Given()
            {
                _tree = new TreeNode();
            }

            [Test]
            public void DepthIsZero()
            {
                Assert.AreEqual(0, _tree.Depth);
            }

            [Test]
            public void ChildrenIsEmpty()
            {
                Assert.IsTrue(!_tree.Children.Any());
            }

            [Test]
            public void ParentIsNull()
            {
                Assert.IsNull(_tree.Parent);
            }

            [Test]
            public void EnumeratesRoot()
            {
                Assert.AreEqual(_tree, _tree.Single());
            }

            [Test]
            public void MaxTreeDepthIsZero()
            {
                Assert.AreEqual(0, _tree.MaximumTreeDepth);
            }

            [Test]
            public void TreeIsRoot()
            {
                Assert.AreEqual(_tree, _tree.Root);
            }
        }

        [TestFixture]
        public class WhenTreeNodeIsInitializedWithDuplicateChildren
        {
            private TreeNode _tree;
            private TreeNode _child;

            [SetUp]
            public void Given()
            {
                _child = new TreeNode();
                _tree = new TreeNode(null, new List<TreeNode> {_child, _child});
            }

            [Test]
            public void OnlyOneChildAdded()
            {
                Assert.AreEqual(_child, _tree.Children.Single());
            }

            [Test]
            public void ChildReferencesRootAsParent()
            {
                Assert.AreEqual(_tree, _child.Parent);
            }

            [Test]
            public void EnumeratesRootAndChild()
            {
                var nodes = _tree.ToList();
                Assert.AreEqual(2, nodes.Count);
                Assert.AreEqual(_tree, nodes.First());
                Assert.AreEqual(_child, nodes.Last());
            }

            [Test]
            public void MaxTreeDepthIsOne()
            {
                Assert.AreEqual(1, _tree.MaximumTreeDepth);
            }

            [Test]
            public void MaxTreeDepthIsZero()
            {
                Assert.AreEqual(1, _tree.MaximumTreeDepth);
            }

            [Test]
            public void TreeIsRoot()
            {
                Assert.AreEqual(_tree, _tree.Root);
            }

            [Test]
            public void TreeIsRootOfChild()
            {
                Assert.AreEqual(_tree, _child.Root);
            }
        }

        [TestFixture]
        public class WhenTreeNodeIsInitializedWithChildren
        {
            private TreeNode _tree;
            private TreeNode _child1;
            private TreeNode _child2;

            [SetUp]
            public void Given()
            {
                _child1 = new TreeNode();
                _child2 = new TreeNode();
                _tree = new TreeNode(null, new List<TreeNode> {_child1, _child2});
            }

            [Test]
            public void TreeContainsAllSpecifiedChildren()
            {
                CollectionAssert.AreEquivalent(new[] {_child1, _child2}, _tree.Children);
            }

            [Test]
            public void ChildReferencesRootAsParent()
            {
                Assert.AreEqual(_tree, _child1.Parent);
                Assert.AreEqual(_tree, _child2.Parent);
            }

            [Test]
            public void ChildHasDepthEqualToOne()
            {
                Assert.AreEqual(1, _child1.Depth);
                Assert.AreEqual(1, _child2.Depth);
            }

            [Test]
            public void EnumeratesRootAndChildren()
            {
                var nodes = _tree.ToList();
                Assert.AreEqual(3, nodes.Count);
                Assert.AreEqual(_tree, nodes.First());
                Assert.AreEqual(_child1, nodes.Skip(1).First());
                Assert.AreEqual(_child2, nodes.Last());
            }

            [Test]
            public void MaxTreeDepthIsZero()
            {
                Assert.AreEqual(1, _tree.MaximumTreeDepth);
            }

            [Test]
            public void TreeIsRootOfChild()
            {
                Assert.AreEqual(_tree, _child1.Root);
                Assert.AreEqual(_tree, _child2.Root);
            }
        }

        [TestFixture]
        public class WhenComplexTreeNodeIsInitialized
        {
            private TreeNode<int> _tree;

            [SetUp]
            public void Given()
            {
                _tree =
                    TreeNode.Build(0,
                        TreeNode.Build(1),
                        TreeNode.Build(2,
                            TreeNode.Build(3),
                            TreeNode.Build(4),
                            TreeNode.Build(5)),
                        TreeNode.Build(6,
                            TreeNode.Build(7,
                                TreeNode.Build(8))),
                        TreeNode.Build(9));
            }

            [Test]
            public void EnumeratesInOrder()
            {
                var nodes = _tree.OfType<TreeNode<int>>().ToList();
                for (int i = 0; i < nodes.Count; i++)
                {
                    Assert.AreEqual(i, nodes[i].Data);
                }
            }

            [Test]
            public void MaxTreeDepthIsThree()
            {
                Assert.AreEqual(3, _tree.MaximumTreeDepth);
            }
        }
    }
}
