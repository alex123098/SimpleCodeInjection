using System;

namespace CodeInjection
{
    public interface IProxyFactory
    {
        Type CreateProxyType<T>(T realInstance, IInjectedPipeline injectedPipeline);
    }
}