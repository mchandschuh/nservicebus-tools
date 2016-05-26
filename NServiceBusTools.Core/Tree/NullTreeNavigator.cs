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

    public static class TreeNavigator
    {
        public static readonly ITreeNavigator AlwaysNavigateDeeper;
        public static readonly ITreeNavigator NeverNavigateDeeper;

        static TreeNavigator()
        {
            AlwaysNavigateDeeper = new ConstantTreeNavigator(true);
            NeverNavigateDeeper = new ConstantTreeNavigator(false);
        }
        private sealed class ConstantTreeNavigator:ITreeNavigator
        {
            private readonly bool _value;

            public ConstantTreeNavigator(bool value)
            {
                _value = value;
            }

            public bool NavigateDeeper(TreeNode node)
            {
                return _value;
            }
        }
    }
}