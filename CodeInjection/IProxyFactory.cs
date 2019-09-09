using System;
using System.Diagnostics.Contracts;

namespace CodeInjection
{
    [ContractClass(typeof(ProxyFactoryContract))]
    public interface IProxyFactory
    {
        Type CreateProxyType<T>(T realInstance, IInjectedPipeline injectedPipeline);
    }

    [ContractClassFor(typeof(IProxyFactory))]
    public abstract class ProxyFactoryContract : IProxyFactory
    {
        public Type CreateProxyType<T>(T realInstance, IInjectedPipeline injectedPipeline)
        {
            Contract.Requires(realInstance != null);
            Contract.Requires(injectedPipeline != null);
            Contract.Ensures(Contract.Result<Type>() != null);
            Contract.Ensures(typeof(T).IsAssignableFrom(Contract.Result<Type>()));
            throw new NotImplementedException();
        }
    }
}