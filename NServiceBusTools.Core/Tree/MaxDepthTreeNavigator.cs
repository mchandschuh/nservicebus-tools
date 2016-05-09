namespace NServiceBusTools.Tree
{
    /// <summary>
    /// Provides an instance of <see cref="ITreeNavigator"/> that will never
    /// navigate deeper than the specified max depth
    /// </summary>
    public class MaxDepthTreeNavigator : ITreeNavigator
    {
        private readonly int _maxDepth;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxDepthTreeNavigator"/> class
        /// </summary>
        /// <param name="maxDepth">The maximum navigation depth</param>
        public MaxDepthTreeNavigator(int maxDepth)
        {
            _maxDepth = maxDepth;
        }

        public bool NavigateDeeper(TreeNode node)
        {
            return node.Depth < _maxDepth;
        }
    }
}