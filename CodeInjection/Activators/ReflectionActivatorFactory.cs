using System;

namespace CodeInjection.Activators
{
    public class ReflectionActivatorFactory : IActivatorFactory
    {
        public FactoryMethodDelegate<T> CreateActivatorOf<T>(Type exactType)
        {
            if (exactType == null) throw new ArgumentNullException(nameof(exactType));

            return (pipeline, instance) => (T) Activator.CreateInstance(exactType, pipeline, instance);
        }
    }
}