using Dapr.Client;
using System;


namespace Testcontainers.Dapr;

public sealed class DaprContainerTest : IAsyncLifetime
{
    private readonly DaprContainer _daprContainer = new DaprBuilder().Build();

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
    public async Task CheckDaprSideCarIsHealthy()
    {
        await Task.Delay(5000);
            
        Environment.SetEnvironmentVariable("DAPR_HTTP_PORT", _daprContainer.DaprHttpPort.ToString());
        Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", _daprContainer.DaprGrpcPort.ToString());

        var logs = await _daprContainer.GetLogsAsync();


        Assert.Equal(3500, _daprContainer.DaprHttpPort);

        // Assert.Equal("",)

        var _daprClient = new DaprClientBuilder().Build();

        var healthy = await _daprClient.CheckHealthAsync();

        Assert.True(healthy, "status : " + healthy);
        //Assert.True(true);
    }
}