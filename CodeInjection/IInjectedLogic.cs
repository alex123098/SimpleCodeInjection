using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace CodeInjection
{
    [ContractClass(typeof(InjectedLogicContract))]
    public interface IInjectedLogic
    {
        IInjectedLogic BeforeExecute(object target, MethodInfo invokedMethod, object[] parameters, IInjectedLogic next);

        IInjectedLogic AfterExecute(object target, MethodInfo invokedMethod, object[] parameters,
            IInjectedLogic previous);
    }

    [ContractClassFor(typeof(IInjectedLogic))]
    public abstract class InjectedLogicContract : IInjectedLogic
    {
        public IInjectedLogic BeforeExecute(object target, MethodInfo invokedMethod, object[] parameters,
            IInjectedLogic next)
        {
            Contract.Requires(target != null);
            Contract.Requires(invokedMethod != null);
            Contract.Requires(parameters != null);
            throw new NotImplementedException();
        }

        public IInjectedLogic AfterExecute(object target, MethodInfo invokedMethod, object[] parameters,
            IInjectedLogic previous)
        {
            Contract.Requires(target != null);
            Contract.Requires(invokedMethod != null);
            Contract.Requires(parameters != null);
            throw new NotImplementedException();
        }
    }
}