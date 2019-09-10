using System;
using System.Collections.Concurrent;
using JetBrains.Annotations;

namespace CodeInjection.Caching
{
    public class CacheableActivatorFactory : IActivatorFactory
    {
        private readonly ConcurrentDictionary<string, object> _cachedFactories;
        private readonly IActivatorFactory _decoratedActivatorFactory;

        public CacheableActivatorFactory(IActivatorFactory activatorFactory)
        {
            _decoratedActivatorFactory = activatorFactory ?? throw new ArgumentNullException(nameof(activatorFactory));
            _cachedFactories = new ConcurrentDictionary<string, object>();
        }

        public FactoryMethodDelegate<T> CreateActivatorOf<T>([NotNull] Type exactType)
        {
            if (exactType == null) throw new ArgumentNullException(nameof(exactType));

            var key = GetKeyFor(typeof(T), exactType);
            if (_cachedFactories.TryGetValue(key, out var cachedFactory)) return (FactoryMethodDelegate<T>) cachedFactory;

            cachedFactory = _decoratedActivatorFactory.CreateActivatorOf<T>(exactType);
            _cachedFactories.TryAdd(key, cachedFactory);
            return (FactoryMethodDelegate<T>) cachedFactory;
        }

        private string GetKeyFor([NotNull] Type abstractType, [NotNull] Type exactType)
        {
            return string.Concat(abstractType.FullName, "##", exactType.FullName);
        }
    }
}