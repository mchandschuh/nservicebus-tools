namespace NServiceBusTools.Tree
{
    /// <summary>
    /// Defines operations for navigating a tree structure
    /// </summary>
    public interface ITreeNavigator
    {
        /// <summary>
        /// Returns true if we should travel deeper into the tree by
        /// interrogating the specified node's children
        /// </summary>
        /// <param name="node">The node to check for navigation</param>
        /// <returns>True to navigate into the specified node's children, false otherwise</returns>
        bool NavigateDeeper(TreeNode node);
    }
}