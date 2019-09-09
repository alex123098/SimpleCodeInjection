namespace CodeInjection
{
    public interface ILogicInjector
    {
        IProxyFactory ProxyFactory { get; set; }
        IActivatorFactory ActivatorFactory { get; set; }
        T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline);
    }
}