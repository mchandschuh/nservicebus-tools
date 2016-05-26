using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NServiceBus;
using NServiceBusTools.Linq;
using NServiceBusTools.Reflection;
using NServiceBusTools.Sample;
using NServiceBusTools.Tree;

namespace NServiceBusTools
{
    class Program
    {
        static void Main(string[] args)
        {
            var searchTypes = new List<Type>
            {
                typeof(IBus),
                typeof(ISendOnlyBus)
            };

            var target = new AnalysisTarget(typeof(Sample.Program).Assembly, typeof(Sample.Program), Reflect.Method<OrderHandler>(o => o.Handle(null)));
            var analyzer = new MessageFlowAnalyzer(target, new AnalysisParameters(searchTypes));
            analyzer.Analyze();
        }
    }

    public class MessageFlowAnalyzer
    {
        private readonly AnalysisTarget target;
        private readonly AnalysisParameters parameters;

        public MessageFlowAnalyzer(AnalysisTarget target, AnalysisParameters parameters)
        {
            this.target = target;
            this.parameters = parameters;
        }

        public TreeNode<string> Analyze()
        {
            var searchTypes = parameters.SearchTypes.Select(x => x.AsTypeDefinition().FullName).ToHashSet();
            var methodDefinition = target.Method.AsMethodDefinition();
            var callTree = methodDefinition.GetCallTree();

            callTree = callTree.Where(x =>
                x.AsEnumerable()
                    .OfType<TreeNode<MethodDefinition>>()
                    .Any(c => searchTypes.Contains(c.Data.DeclaringType.FullName))
                );

            callTree.WriteTo(Console.Out);
            // for now just reproduce the names of each method called
            return callTree.Select(x => new TreeNode<string>(x.Parent, x.Data.FullName, x.Children()));
        }
    }

    public class AnalysisParameters
    {
        public HashSet<Type> SearchTypes { get; private set; }

        public AnalysisParameters(IEnumerable<Type> searchTypes)
        {
            SearchTypes = searchTypes.ToHashSet();
        }
    }

    public class AnalysisTargetInputs
    {
        public string AssemblyLocation { get; private set; }
        public string Type { get; private set; }
        public string Method { get; private set; }

        public AnalysisTargetInputs(string assemblyLocation, string type, string method)
        {
            AssemblyLocation = assemblyLocation;
            Type = type;
            Method = method;
        }

        public AnalysisTarget ResolveTarget()
        {
            var assembly = Assembly.ReflectionOnlyLoad(AssemblyLocation);
            var type = assembly.GetTypes().Single(x => x.FullName == Type);
            var method = type.GetMethods().Single(x => x.Name == Method);
            return new AnalysisTarget(assembly, type, method);
        }
    }

    public class AnalysisTarget
    {
        public Assembly Assembly { get; private set; }
        public Type Type { get; private set; }
        public MethodInfo Method { get; private set; }
        
        public AnalysisTarget(Assembly assembly, Type type, MethodInfo method)
        {
            Assembly = assembly;
            Type = type;
            Method = method;
        }
    }
}
