using System.Reflection;

namespace CodeInjection
{
    public interface IInjectedPipeline
    {
        void Add(IInjectedLogic injectedLogic);
        void ExecutePreCondition(object target, MethodInfo invokedMethodInfo, object[] arguments);
        void ExecutePostCondition(object target, MethodInfo invokedMethodInfo, object[] arguments);
    }
}