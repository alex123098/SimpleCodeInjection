using System;
using JetBrains.Annotations;

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

        [NotNull]
        public IProxyFactory ProxyFactory
        {
            get => _proxyFactory;
            set => _proxyFactory = value ?? throw new ArgumentNullException();
        }

        [NotNull]
        public IActivatorFactory ActivatorFactory
        {
            get => _activatorFactory;
            set => _activatorFactory = value ?? throw new ArgumentNullException();
        }

        public T CreateProxyFor<T>([NotNull] T realInstance, [NotNull] IInjectedPipeline injectedPipeline)
        {
            if (realInstance == null) throw new ArgumentNullException(nameof(realInstance));
            if (injectedPipeline == null) throw new ArgumentNullException(nameof(injectedPipeline));

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
    }
}