using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using NUnit.Framework;

namespace NServiceBusTools.Reflection
{
    [TestFixture]
    public class ReflectionExtensionsTests
    {
        [Test]
        public void GetCallTreeByDefaultPreventsInfiniteLoop()
        {
            var callTree = CallTreeSampleMethods.SelfReferencingMethodInfo.GetCallTree();
            Assert.AreEqual(nameof(CallTreeSampleMethods.InfiniteLoop), callTree.Data.Name);

            Console.WriteLine(callTree);
        }

        [Test]
        public void GetCallTreeDateTimeNow()
        {
            var methodInfo = typeof (DateTime).GetMethod("get_" + nameof(DateTime.Now));
            var callTree = methodInfo.GetCallTree();
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
