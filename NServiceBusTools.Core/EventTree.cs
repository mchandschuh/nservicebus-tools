using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NServiceBusTools.Reflection;
using NServiceBusTools.Tree;

namespace NServiceBusTools
{
    public static class EventTree
    {
        public static TreeNode<Event> Build(MethodInfo entry)
        {
            var methodDefinition = entry.AsMethodDefinition();
            return TreeNode.Build(CreateEvent(methodDefinition, "entry"), TreeNavigator.AlwaysNavigateDeeper, node => ChildrenFactory(node.Data));
        }

        private static bool NavigateDeeper(TreeNode<MethodDefinition> node)
        {
            var assemblyFullName = node.Data.DeclaringType?.Namespace;
            if (assemblyFullName == null)
            {
                return false;
            }
            return ContinueByNamespace(assemblyFullName);
        }

        private static bool ContinueByNamespace(string @namespace)
        {
            if (@namespace.StartsWith("System."))
            {
                return false;
            }
            if (@namespace.StartsWith("NServiceBus"))
            {
                return false;
            }
            return true;
        }

        private static IEnumerable<Event> ChildrenFactory(Event @event)
        {
            var methodDefinition = @event.ExecutingMethod;
            var treeNavigator = new FuncTreeNavigator(node => NavigateDeeper((TreeNode<MethodDefinition >) node));
            var callTree = methodDefinition.GetCallTree(treeNavigator);
            var nServiceBusCalls = callTree.Where(node =>
                node.Data.DeclaringType.Namespace.StartsWith("NServiceBus")
                );
            return nServiceBusCalls.AsEnumerable().Select(node => CreateEvent(node.Data, methodDefinition.Name));
        }

        private static Event CreateEvent(MethodDefinition methodDefinition, string eventType)
        {
            return new Event(methodDefinition, eventType);
        }
    }

    public class Event
    {
        public readonly string EventType;
        public readonly MethodDefinition ExecutingMethod;

        public Event(MethodDefinition executingMethod, string eventType)
        {
            EventType = eventType;
            ExecutingMethod = executingMethod;
        }
    }

    public enum EventType
    {
        Send,
        SendLocal,
        Publish
    }
}
