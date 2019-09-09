using System;
using JetBrains.Annotations;

namespace CodeInjection
{
    public class ReflectionActivatorFactory : IActivatorFactory
    {
        public FactoryMethodDelegate<T> CreateActivatorOf<T>([NotNull] Type exactType)
        {
            if (exactType == null) throw new ArgumentNullException(nameof(exactType));

            return (pipeline, instance) => (T) Activator.CreateInstance(exactType, pipeline, instance);
        }
    }
}