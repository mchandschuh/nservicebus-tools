using System;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

namespace NServiceBusTools.Reflection
{
    public static class MonoCecilBridgeTests
    {
        private static readonly MethodInfo DateTimeAddDays = typeof(DateTime).GetMethod(nameof(DateTime.AddDays));

        [TestFixture]
        public class WhenAsMethodDefinitionIsCalledWithMethodInfoWithDeclaringType
        {
            private MethodInfo _input;
            private MethodDefinition _result;

            [SetUp]
            public void Given()
            {
                _input = DateTimeAddDays;
                _result = _input.AsMethodDefinition();
            }

            [Test]
            public void ResultHasEqualParameters()
            {
                var parameters = _input.GetParameters().Select(x => x.ParameterType.FullName);
                var cecilParameters = _result.Parameters.Select(x => x.ParameterType.FullName);
                CollectionAssert.AreEqual(parameters, cecilParameters);
            }

            [Test]
            public void ResultIsEquivalentToInput()
            {
                Assert.IsTrue(_result.IsEquivalentTo(_input));
            }
        }

        [TestFixture]
        public class WhenIsEquivalentToIsCalledWithEquivalentMethods
        {
            private bool _result;
            private MethodInfo _methodInfo;
            private MethodDefinition _methodDefinition;

            [SetUp]
            public void Given()
            {
                _methodInfo = DateTimeAddDays;
                _methodDefinition = _methodInfo.AsMethodDefinition();
                _result = _methodInfo.IsEquivalentTo(_methodDefinition);
            }

            [Test]
            public void ResultIsTrue()
            {
                Assert.IsTrue(_result);
            }
        }
    }
}
