using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;

namespace CodeInjection
{
	public class CacheableActivatorFactory : IActivatorFactory
	{
		private readonly IActivatorFactory _decoratedActivatorFactory;
		private readonly ConcurrentDictionary<string, object> _cachedFactories;

		public CacheableActivatorFactory(IActivatorFactory activatorFactory) {
			Contract.Requires(activatorFactory != null);
			_decoratedActivatorFactory = activatorFactory;
			_cachedFactories = new ConcurrentDictionary<string, object>();
		}

		public FactoryMethodDelegate<T> CreateActivatorOf<T>(Type exactType) {
			object cachedFactory;
			var key = GetKeyFor(typeof (T), exactType);
			if (_cachedFactories.TryGetValue(key, out cachedFactory)) {
				return (FactoryMethodDelegate<T>) cachedFactory;
			}
			cachedFactory = _decoratedActivatorFactory.CreateActivatorOf<T>(exactType);
			_cachedFactories.TryAdd(key, cachedFactory);
			return (FactoryMethodDelegate<T>) cachedFactory;
		}

		private string GetKeyFor(Type abstractType, Type exactType) {
			Contract.Requires(abstractType != null);
			Contract.Requires(exactType != null);
			Contract.Ensures(Contract.Result<string>() != null);

			return string.Concat(abstractType.FullName, "##", exactType.FullName);
		}

		[ContractInvariantMethod]
		private void ContractInvariants() {
			Contract.Invariant(_decoratedActivatorFactory != null);
			Contract.Invariant(_cachedFactories != null);
		}
	}
}
