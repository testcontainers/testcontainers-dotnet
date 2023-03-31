using Dapr.Client;
using System;

namespace Testcontainers.Dapr;

public sealed class DaprContainerTest : IAsyncLifetime
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
        Environment.SetEnvironmentVariable("DAPR_HTTP_PORT", _daprContainer.DaprHttpPort.ToString());
        Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", _daprContainer.DaprGrpcPort.ToString());

        var _daprClient = new DaprClientBuilder().Build();
        var healthy = await _daprClient.CheckHealthAsync();

        Assert.True(true);

        //use this to hold the container in an active state for debugging
        //await Task.Delay(TimeSpan.FromHours(1));
    }
}