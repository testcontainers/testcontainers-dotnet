namespace Testcontainers.Dapr;

public sealed class SmokeTest : IAsyncLifetime
{
    private DaprContainer _daprContainer;

    public Task InitializeAsync()
    {
        _daprContainer = new DaprBuilder()
            .WithAppId("testicorns")
            .WithLogLevel("debug")
            .Build();

        return _daprContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _daprContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CheckDaprSideCarIsHealthy()
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
        _appContainer = new ContainerBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithNetwork(_network)
            .WithImage("nginx")
            .WithPortBinding(appPort, true)
            .Build();

        await _appContainer.StartAsync().ConfigureAwait(false);

        _daprContainer = new DaprBuilder()
            .WithAppId("testicorns")
            .WithAppPort(_appContainer.GetMappedPublicPort(appPort))
            .WithLogLevel("debug")
            .WithNetwork(_network)
            .DependsOn(_appContainer)
            .Build();

        await _daprContainer.StartAsync().ConfigureAwait(false);
        // this never completes. Dapr is blocking while waiting for the app-port to be ready on 127.0.0.1. 
        // Will be fixed by https://github.com/dapr/dapr/issues/6177
    }

    public async Task DisposeAsync()
    {
        await _daprContainer.DisposeAsync().ConfigureAwait(false);
        await _appContainer.DisposeAsync().ConfigureAwait(false);
        await _network.DeleteAsync().ConfigureAwait(false);
    }

    [Fact(Skip = "Disable this test from running CI automatically, as it will just hang forever")]
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