using Dapr.Client;

namespace Testcontainers.Dapr;

public sealed class DaprContainerTest : IAsyncLifetime
{
    private readonly DaprContainer _daprContainer = new DaprBuilder().Build();

    public async Task InitializeAsync()
    {
        var c = await _daprContainer.StartAsync();
        Environment.SetEnvironmentVariable("DAPR_HTTP_PORT", _daprContainer.DaprHttpPort.ToString());
        Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", _daprContainer.DaprHttpPort.ToString());
        return c;
    }

    public Task DisposeAsync()
    {
        return _daprContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CheckDaprSideCarIsHealthy()
    {
        var _daprClient = new DaprClientBuilder().Build();

        var healthy = await _daprClient.CheckHealthAsync().ConfigureAwait(false);

        Assert.True(healthy);
    }
}