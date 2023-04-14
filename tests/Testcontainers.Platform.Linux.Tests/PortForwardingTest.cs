namespace Testcontainers.Tests;

public sealed class PortForwardingTest : IAsyncLifetime
{
    private readonly IContainer _container = new ContainerBuilder()
        .WithImage(CommonImages.Curl)
        .WithExtraHost("host.testcontainers.internal", null)
        .Build();

    public Task InitializeAsync()
    {
        return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _container.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public Task EstablishesHostConnection()
    {
        return Task.CompletedTask;
    }
}