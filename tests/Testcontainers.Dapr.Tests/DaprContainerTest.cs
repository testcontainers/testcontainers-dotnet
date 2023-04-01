using Dapr.Client;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using System;

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
        Environment.SetEnvironmentVariable("DAPR_HTTP_PORT", _daprContainer.DaprHttpPort.ToString());
        Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", _daprContainer.DaprGrpcPort.ToString());

        var _daprClient = new DaprClientBuilder().Build();
        var healthy = await _daprClient.CheckHealthAsync();

        Assert.True(healthy);
    }
}

public sealed class AppPortTests : IAsyncLifetime
{
    private DaprContainer _daprContainer;

    private IContainer _nginxContainer;

    private INetwork _network;

    public async Task InitializeAsync()
    {
        _network = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .Build();

        await _network.CreateAsync().ConfigureAwait(false);

        const ushort NginxHttpPort = 80;
        _nginxContainer = new ContainerBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .WithNetwork(_network)
            .WithImage("nginx")
            .WithCommand("nginx-debug", "-g", "daemon off;")
            .WithPortBinding(NginxHttpPort, true)
            .Build();

        await _nginxContainer.StartAsync().ConfigureAwait(false);

        _daprContainer = new DaprBuilder()
            .WithAppId("testicorns")
            .WithAppPort(_nginxContainer.GetMappedPublicPort(NginxHttpPort))
            .WithLogLevel("debug")
            .WithNetwork(_network)
            .Build();

        await _daprContainer.StartAsync().ConfigureAwait(false);
        // this never completes. Dapr is blocking while waiting for the nginx port to be ready. Not sure why dapr can't communicate with it.
    }

    public async Task DisposeAsync()
    {
        await _daprContainer.DisposeAsync().ConfigureAwait(false);
        await _nginxContainer.DisposeAsync().ConfigureAwait(false);
        await _network.DeleteAsync().ConfigureAwait(false);
    }


    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task AppPortCanBeReached()
    {            

        Environment.SetEnvironmentVariable("DAPR_HTTP_PORT", _daprContainer.DaprHttpPort.ToString());
        Environment.SetEnvironmentVariable("DAPR_GRPC_PORT", _daprContainer.DaprGrpcPort.ToString());

        var _daprClient = new DaprClientBuilder().Build();
        var healthy = await _daprClient.CheckHealthAsync();

        Assert.True(healthy);
    }
}