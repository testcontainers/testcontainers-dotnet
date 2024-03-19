namespace DotNet.Testcontainers.Commons;

[PublicAPI]
public abstract class SharedContainerInstance<TContainer> : IAsyncLifetime
    where TContainer : IContainer
{
    public SharedContainerInstance(TContainer container)
    {
        Container = container;
    }

    public TContainer Container { get; }

    public Task InitializeAsync()
    {
        return Container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return Container.DisposeAsync().AsTask();
    }
}