using System;
using System.Reflection;
using NServiceBusTools.Tree;
using NUnit.Framework;

namespace NServiceBusTools.Reflection
{
    [TestFixture]
    public class MethodCallTreeTests
    {
        [Test]
        public void Test()
        {
            var ifElse = Reflect.Method(() => CallTreeSampleMethods.IfElse(default(bool))).AsMethodDefinition();
            var tree = ifElse.GetCallTree();

            tree.WriteTo(Console.Out);
        }
    }

    [TestFixture]
    public class ReflectionExtensionsTests
    {
        [Test]
        public void GetCallTreeByDefaultPreventsInfiniteLoop()
        {
            var callTree = Reflect.Method(() => CallTreeSampleMethods.InfiniteLoop()).AsMethodDefinition().GetCallTree();
            Assert.AreEqual(nameof(CallTreeSampleMethods.InfiniteLoop), callTree.Data.Name);

            Console.WriteLine(callTree);
        }

        [Test]
        public void GetCallTreeDateTimeNow()
        {
            var maxDepthNavigator = new MaxDepthTreeNavigator(5);
            var methodInfo = Reflect.Property(() => DateTime.Now).GetMethod;
            var methodDefinition = methodInfo.AsMethodDefinition();
            var callTree = methodDefinition.GetCallTree(maxDepthNavigator);
            callTree.WriteTo(Console.Out);
        }
    }

    public static class CallTreeSampleMethods
    {
        public static void InfiniteLoop()
        {
            InfiniteLoop();
        }

        public static void CallsOneMethod()
        {
            NoOp();
        }

        public static void NoOp()
        {
        }

        public static int IfElse(bool condition)
        {
            if (condition)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }
}
