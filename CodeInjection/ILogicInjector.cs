using System;
using System.Diagnostics.Contracts;

namespace CodeInjection
{
    [ContractClass(typeof(LogicInjectorContract))]
    public interface ILogicInjector
    {
        IProxyFactory ProxyFactory { get; set; }
        IActivatorFactory ActivatorFactory { get; set; }
        T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline);
    }

    [ContractClassFor(typeof(ILogicInjector))]
    public abstract class LogicInjectorContract : ILogicInjector
    {
        public T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline)
        {
            Contract.Requires(realInstance != null);
            Contract.Requires(injectedPipeline != null);
            Contract.Ensures(Contract.Result<T>() != null);
            throw new NotImplementedException();
        }

        public IProxyFactory ProxyFactory
        {
            [Pure]
            get
            {
                Contract.Ensures(Contract.Result<IProxyFactory>() != null);
                throw new NotImplementedException();
            }
            set
            {
                Contract.Requires(value != null);
                throw new NotImplementedException();
            }
        }

        public IActivatorFactory ActivatorFactory
        {
            [Pure]
            get
            {
                Contract.Ensures(Contract.Result<IActivatorFactory>() != null);
                throw new NotImplementedException();
            }
            set
            {
                Contract.Requires(value != null);
                throw new NotImplementedException();
            }
        }
    }
}