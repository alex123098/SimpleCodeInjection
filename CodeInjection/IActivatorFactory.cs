using System;

namespace CodeInjection
{
    public delegate object FactoryMethodDelegate<in T>(IInjectedPipeline pipeline, T realInstance);

    public interface IActivatorFactory
    {
        FactoryMethodDelegate<T> CreateActivatorOf<T>(Type exactType);
    }
}