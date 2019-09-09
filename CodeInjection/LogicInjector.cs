using System.Diagnostics.Contracts;

namespace CodeInjection
{
    public class LogicInjector : ILogicInjector
    {
        private IActivatorFactory _activatorFactory;
        private IProxyFactory _proxyFactory;

        public LogicInjector()
        {
            Contract.Ensures(ProxyFactory != null);

            SetDefaultProxyFactory();
            SetDefaultActivatorFactory();
        }

        public IProxyFactory ProxyFactory
        {
            [Pure]
            get
            {
                Contract.Ensures(Contract.Result<IProxyFactory>() != null);
                return _proxyFactory;
            }
            set => _proxyFactory = value;
        }

        public IActivatorFactory ActivatorFactory
        {
            [Pure]
            get
            {
                Contract.Ensures(Contract.Result<IActivatorFactory>() != null);
                return _activatorFactory;
            }
            set => _activatorFactory = value;
        }

        public T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline)
        {
            var proxyType = ProxyFactory.CreateProxyType(realInstance, injectedPipeline);
            var activator = ActivatorFactory.CreateActivatorOf<T>(proxyType);
            return (T) activator.Invoke(injectedPipeline, realInstance);
        }

        private void SetDefaultProxyFactory()
        {
            _proxyFactory = new CacheableProxyFactory(new ProxyFactory());
        }

        private void SetDefaultActivatorFactory()
        {
            _activatorFactory = new CacheableActivatorFactory(new DynamicActivatorFactory());
        }

        [ContractInvariantMethod]
        private void ContractInvariants()
        {
            Contract.Invariant(_proxyFactory != null);
            Contract.Invariant(_activatorFactory != null);
        }
    }
}