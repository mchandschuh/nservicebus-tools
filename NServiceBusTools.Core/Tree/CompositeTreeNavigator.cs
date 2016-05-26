using System;

namespace NServiceBusTools.Tree
{
    public class CompositeTreeNavigator : ITreeNavigator
    {
        private readonly ITreeNavigator first;
        private readonly ITreeNavigator second;

        public CompositeTreeNavigator(ITreeNavigator first, ITreeNavigator second)
        {
            this.first = first;
            this.second = second;
        }
        public bool NavigateDeeper(TreeNode node)
        {
            if (first.NavigateDeeper(node))
            {
                return second.NavigateDeeper(node);
            }
            return false;
        }
    }

    public class FuncTreeNavigator : ITreeNavigator
    {
        private readonly Func<TreeNode, bool> navigateDeeper;

        public FuncTreeNavigator(Func<TreeNode, bool> navigateDeeper)
        {
            this.navigateDeeper = navigateDeeper;
        }

        public bool NavigateDeeper(TreeNode node)
        {
            return navigateDeeper(node);
        }
    }
}
