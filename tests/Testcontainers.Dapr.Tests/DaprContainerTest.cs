namespace Testcontainers.Dapr;

public sealed class DaprContainerTest : IAsyncLifetime
{
    private readonly DaprContainer _daprContainer;

    public DaprContainerTest()
    {
        var network = new NetworkBuilder()
            .Build();

        var nginxContainer = new ContainerBuilder()
            .WithImage(CommonImages.Nginx)
            .WithNetwork(network)
            .WithNetworkAliases("nginx")
            .Build();

        _daprContainer = new DaprBuilder()
            .DependsOn(nginxContainer)

            // TODO: Doesn't Dapr establishes a connection to the container via its network alias? How does the Compose configuration work: https://docs.dapr.io/operations/hosting/self-hosted/self-hosted-with-docker/#run-using-docker-compose?
            .WithCommand("--app-id", "nginx")

            // TODO: This line prevents daprd from starting, it says:
            // application protocol: http. waiting on port 80.  This will block until the app is listening on that port." app_id=nginx instance=1a23cb66e7f5 scope=dapr.runtime type=log ver=1.10.4
            // .WithCommand("--app-port", "80")

            .WithNetwork(network)
            .WithNetworkAliases("daprd")
            .Build();
    }

    public Task InitializeAsync()
    {
        return _daprContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _daprContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CheckHealthReturnsTrue()
    {
        // Given
        using var client = new DaprClientBuilder()
            .UseHttpEndpoint(_daprContainer.GetHttpAddress())
            .UseGrpcEndpoint(_daprContainer.GetGrpcAddress())
            .Build();

        // When
        var healthy = await client.CheckHealthAsync()
            .ConfigureAwait(false);

        // Then
        Assert.True(healthy);
    }
}