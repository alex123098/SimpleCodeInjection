using System;
using CodeInjection.Activators;
using CodeInjection.Caching;

namespace CodeInjection
{
    public class LogicInjector : ILogicInjector
    {
        private IActivatorFactory _activatorFactory;
        private IProxyFactory _proxyFactory;

        public LogicInjector()
        {
            SetDefaultProxyFactory();
            SetDefaultActivatorFactory();
        }

        public IProxyFactory ProxyFactory
        {
            get => _proxyFactory;
            set => _proxyFactory = value ?? throw new ArgumentNullException();
        }

        public IActivatorFactory ActivatorFactory
        {
            get => _activatorFactory;
            set => _activatorFactory = value ?? throw new ArgumentNullException();
        }

        public TResult CreateProxyFor<T, TResult>(T realInstance, IInjectedPipeline injectedPipeline) where T : TResult
        {
            if (realInstance == null) throw new ArgumentNullException(nameof(realInstance));
            if (injectedPipeline == null) throw new ArgumentNullException(nameof(injectedPipeline));

            var proxyType = ProxyFactory.CreateProxyType(realInstance, injectedPipeline);
            var activator = ActivatorFactory.CreateActivatorOf<T>(proxyType);
            return (TResult) activator.Invoke(injectedPipeline, realInstance);
        }

        public T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline)
        {
            return CreateProxyFor<T, T>(realInstance, injectedPipeline);
        }

        private void SetDefaultProxyFactory()
        {
            _proxyFactory = new CacheableProxyFactory(new ProxyFactory());
        }

        private void SetDefaultActivatorFactory()
        {
            _activatorFactory = new CacheableActivatorFactory(new DynamicActivatorFactory());
        }
    }
}