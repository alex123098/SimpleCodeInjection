using System;
using System.Reflection;

namespace CodeInjection
{
    public interface IInjectedLogic
    {
        IInjectedLogic BeforeExecute(object target, MethodInfo invokedMethod, object[] parameters, IInjectedLogic next);

        IInjectedLogic ExecutionException(object target, MethodInfo invokedMethod, object[] parameters, Exception e, IInjectedLogic previous);

        IInjectedLogic AfterExecute(object target, MethodInfo invokedMethod, object[] parameters, IInjectedLogic previous);
    }
}