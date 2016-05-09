using NUnit.Framework;

namespace NServiceBusTools.Tree
{
    [TestFixture]
    public class TreeNodeTests
    {
        [Test]
        public void IntuitiveConstructionSyntax()
        {
            // just codifying the syntax
            var tree = new TreeNode
            {
                new TreeNode
                {
                    new TreeNode(),
                    new TreeNode(),
                    new TreeNode()
                },
                new TreeNode
                {
                    new TreeNode(),
                    new TreeNode()
                }
            };
        }

        [Test]
        public void CurrentDepthZeroOnRootNode()
        {
            var tree = new TreeNode {new TreeNode {new TreeNode()}};
            Assert.AreEqual(0, tree.Depth);
        }
    }
}
