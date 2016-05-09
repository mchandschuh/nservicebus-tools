namespace NServiceBusTools.Tree
{
    /// <summary>
    /// Provides an instance of <see cref="ITreeNavigator"/> that will
    /// always navigate deeper
    /// </summary>
    public sealed class NullTreeNavigator : ITreeNavigator
    {
        public static readonly NullTreeNavigator Instance = new NullTreeNavigator();

        private NullTreeNavigator()
        {
        }

        public bool NavigateDeeper(TreeNode node)
        {
            return true;
        }
    }
}