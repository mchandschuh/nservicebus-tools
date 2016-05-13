using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using NServiceBusTools.Tree;
using NUnit.Framework;

namespace NServiceBusTools.Reflection
{
    [TestFixture]
    public class ReflectionExtensionsTests
    {
        [Test]
        public void GetCallTreeByDefaultPreventsInfiniteLoop()
        {
            var callTree = CallTreeSampleMethods.SelfReferencingMethodInfo.AsMethodDefinition().GetCallTree();
            Assert.AreEqual(nameof(CallTreeSampleMethods.InfiniteLoop), callTree.Data.Name);

            Console.WriteLine(callTree);
        }

        [Test]
        public void GetCallTreeDateTimeNow()
        {
            var maxDepthNavigator = new MaxDepthTreeNavigator(5);
            var methodInfo = typeof (DateTime).GetMethod("get_" + nameof(DateTime.Now));
            var methodDefinition = methodInfo.AsMethodDefinition();
            var callTree = methodDefinition.GetCallTree(maxDepthNavigator);
            callTree.WriteTo(Console.Out);
        }
    }

    public static class CallTreeSampleMethods
    {
        public static readonly MethodInfo SelfReferencingMethodInfo = typeof (CallTreeSampleMethods).GetMethod(nameof(InfiniteLoop));

        public static void InfiniteLoop()
        {
            InfiniteLoop();
        }
    }
}
