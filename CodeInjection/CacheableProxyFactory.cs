using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;

namespace CodeInjection
{
	public class CacheableProxyFactory : IProxyFactory
	{
		private readonly IProxyFactory _decoratedFactory;
		private readonly ConcurrentDictionary<string, Type> _proxyCache;

		public CacheableProxyFactory(IProxyFactory proxyFactory) {
			Contract.Requires(proxyFactory != null);

			_decoratedFactory = proxyFactory;
			_proxyCache = new ConcurrentDictionary<string, Type>();
		}

		public Type CreateProxyType<T>(T realInstance, IInjectedPipeline injectedPipeline) {
			Type proxyType;
			var instanceType = realInstance.GetType();
			var pipelineType = injectedPipeline.GetType();
			var proxyKey = GetProxyKey(instanceType, pipelineType);

			if (_proxyCache.TryGetValue(proxyKey, out proxyType)) {
				Contract.Assume(typeof(T).IsAssignableFrom(proxyType));
				return proxyType;
			}
			proxyType = _decoratedFactory.CreateProxyType(realInstance, injectedPipeline);
			_proxyCache.TryAdd(proxyKey, proxyType);
			return proxyType;
		}

		private string GetProxyKey(Type instanceType, Type pipelineType) {
			Contract.Assume(instanceType != null);
			Contract.Assume(pipelineType != null);

			return string.Concat(instanceType.FullName, "##", pipelineType.FullName);
		}

		[ContractInvariantMethod]
		private void ContractInvariants() {
			Contract.Invariant(_decoratedFactory != null);
			Contract.Invariant(_proxyCache != null);
		}
	}
}
