namespace CodeInjection
{
    public interface ILogicInjector
    {
        IProxyFactory ProxyFactory { get; set; }
        IActivatorFactory ActivatorFactory { get; set; }
        T CreateProxyFor<T>(T realInstance, IInjectedPipeline injectedPipeline);
        TResult CreateProxyFor<T, TResult>(T realInstance, IInjectedPipeline injectedPipeline) where T : TResult;
    }
}