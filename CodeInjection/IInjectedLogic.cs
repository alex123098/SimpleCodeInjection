using System.Reflection;

namespace CodeInjection
{
    public interface IInjectedLogic
    {
        IInjectedLogic BeforeExecute(object target, MethodInfo invokedMethod, object[] parameters, IInjectedLogic next);

        IInjectedLogic AfterExecute(object target, MethodInfo invokedMethod, object[] parameters, IInjectedLogic previous);
    }
}