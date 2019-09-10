using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;

namespace CodeInjection.Caching
{
    public class CacheableProxyFactory : IProxyFactory
    {
        private readonly IProxyFactory _decoratedFactory;
        private readonly ConcurrentDictionary<string, Type> _proxyCache;

        public CacheableProxyFactory(IProxyFactory proxyFactory)
        {
            _decoratedFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            _proxyCache = new ConcurrentDictionary<string, Type>();
        }

        public Type CreateProxyType<T>(T realInstance, IInjectedPipeline injectedPipeline)
        {
            if (realInstance == null) throw new ArgumentNullException(nameof(realInstance));
            if (injectedPipeline == null) throw new ArgumentNullException(nameof(injectedPipeline));

            var instanceType = realInstance.GetType();
            var pipelineType = injectedPipeline.GetType();
            var proxyKey = GetProxyKey(instanceType, pipelineType);

            if (_proxyCache.TryGetValue(proxyKey, out var proxyType))
            {
                Contract.Assume(typeof(T).IsAssignableFrom(proxyType));
                return proxyType;
            }

            proxyType = _decoratedFactory.CreateProxyType(realInstance, injectedPipeline);
            _proxyCache.TryAdd(proxyKey, proxyType);
            return proxyType;
        }

        private string GetProxyKey(Type instanceType, Type pipelineType)
        {
            return string.Concat(instanceType.FullName, "##", pipelineType.FullName);
        }
    }
}