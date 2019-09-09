using System;

namespace CodeInjection
{
    public class ReflectionActivatorFactory : IActivatorFactory
    {
        public FactoryMethodDelegate<T> CreateActivatorOf<T>(Type exactType)
        {
            return (pipeline, instance) => (T) Activator.CreateInstance(exactType, pipeline, instance);
        }
    }
}