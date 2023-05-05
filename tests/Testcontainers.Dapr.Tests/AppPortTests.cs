namespace Testcontainers.Dapr;

public sealed class AppPortTests : IAsyncLifetime
{
    private DaprContainer _daprContainer;
    private IContainer _appContainer;
    private INetwork _network;
    public async Task InitializeAsync()
    {
        _network = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .Build();

        await _network.CreateAsync().ConfigureAwait(false);

        const ushort appPort = 80;
        string appAlias = $"nginx-{Guid.NewGuid().ToString("D")}";
        _appContainer = new ContainerBuilder()
            .WithName(appAlias)
            .WithNetwork(_network)
            .WithNetworkAliases(appAlias)
            .WithImage("nginx")
            .Build();

        await _appContainer.StartAsync().ConfigureAwait(false);

        _daprContainer = new DaprBuilder()
            .WithAppId("test-app")
            .WithAppChannelAddress(appAlias)
            .WithAppPort(appPort)
            .WithNetwork(_network)
            .DependsOn(_appContainer)
            .Build();

        await _daprContainer.StartAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _daprContainer.DisposeAsync().ConfigureAwait(false);
        await _appContainer.DisposeAsync().ConfigureAwait(false);
        await _network.DeleteAsync().ConfigureAwait(false);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task AppPortCanBeReached()
    {            
        // Given
        using var client = new DaprClientBuilder()
            .UseHttpEndpoint(_daprContainer.GetHttpAddress())
            .UseGrpcEndpoint(_daprContainer.GetGrpcAddress())
            .Build();

        // When
        var healthy = await client.CheckHealthAsync();

        // Then
        Assert.True(healthy);
    }
}