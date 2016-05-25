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
    public class MethodCallTreeTests
    {
        [Test]
        public void Test()
        {
            var ifElse = CallTreeSampleMethods.IfElseMethodInfo.AsMethodDefinition();
            var tree = ifElse.GetCallTree();

            foreach (var instruction in tree.Data.Body.Instructions)
            {
                Console.WriteLine(instruction);
            }
        }
    }

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
            var methodInfo = Reflect.Property(() => DateTime.Now).GetMethod;
            var methodDefinition = methodInfo.AsMethodDefinition();
            var callTree = methodDefinition.GetCallTree(maxDepthNavigator);
            callTree.WriteTo(Console.Out);
        }
    }

    public static class CallTreeSampleMethods
    {
        public static readonly MethodInfo SelfReferencingMethodInfo = typeof (CallTreeSampleMethods).GetMethod(nameof(InfiniteLoop));
        public static readonly MethodInfo IfElseMethodInfo = Reflect.Method(() => IfElse(true));

        public static void InfiniteLoop()
        {
            InfiniteLoop();
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
