using System.Net.Http;
using System.Text;
using System.Threading;
using Testcontainers.Redis;

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

public sealed class ServiceInvocationTests : IAsyncLifetime
{
    private DaprContainer _receiverDaprContainer;
    private DaprContainer _senderDaprContainer;
    private IContainer _receiverAppContainer;
    private INetwork _network;
    private string _daprAppId = "receiver";
    public async Task InitializeAsync()
    {
        _network = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .Build();
        await _network.CreateAsync().ConfigureAwait(false);

        const ushort receiverAppPort = 80;
        string receiverAppNetworkAlias = "receiver-app";
        _receiverAppContainer = new ContainerBuilder()
            .WithName("receiver-app")
            .WithNetwork(_network)
            .WithNetworkAliases(receiverAppNetworkAlias)
            .WithImage("nginx")
            .WithExposedPort(receiverAppPort)
            .WithResourceMapping($"nginx/nginx.conf", "/etc/nginx/nginx.conf")
            .Build();
        await _receiverAppContainer.StartAsync().ConfigureAwait(false);

        _receiverDaprContainer = new DaprBuilder()
            .WithName("receiver-dapr")
            .WithAppId(_daprAppId)
            .WithAppChannelAddress(receiverAppNetworkAlias)
            .WithAppPort(receiverAppPort)
            .WithNetwork(_network)
            .DependsOn(_receiverAppContainer)
            .Build();
        await _receiverDaprContainer.StartAsync().ConfigureAwait(false);

        _senderDaprContainer = new DaprBuilder()
            .WithName("sender-dapr")
            .WithAppId("sender")
            .WithNetwork(_network)
            .DependsOn(_receiverDaprContainer)
            .Build();
        await _senderDaprContainer.StartAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _senderDaprContainer.DisposeAsync().ConfigureAwait(false);
        await _receiverDaprContainer.DisposeAsync().ConfigureAwait(false);
        await _receiverAppContainer.DisposeAsync().ConfigureAwait(false);
        await _network.DeleteAsync().ConfigureAwait(false);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ServiceCanBeCalled()
    {            
        // Given
        using var client = new DaprClientBuilder()
            .UseHttpEndpoint(_senderDaprContainer.GetHttpAddress())
            .UseGrpcEndpoint(_senderDaprContainer.GetGrpcAddress())
            .Build();

        // When
        var healthy = await client.CheckHealthAsync();
        var cts = new CancellationTokenSource();
        var method_A_result = await client.InvokeMethodAsync<string>(HttpMethod.Get, _daprAppId, "method-a", cts.Token);
        var method_B_result = await client.InvokeMethodAsync<string>(HttpMethod.Get, _daprAppId, "method-b", cts.Token);

        // Then
        Assert.Equal("method-a-response", method_A_result);
        Assert.Equal("method-b-response", method_B_result);
    }
}

public sealed class StateManagementTests : IAsyncLifetime
{
    private RedisContainer _redisContainer;
    private DaprContainer _daprContainer;
    private INetwork _network;
    private const string _stateStoreName = "mystatestore";
    private string _redisComponent = @"---
        apiVersion: dapr.io/v1alpha1
        kind: Component
        metadata:
         name: {0}
        spec:
         type: state.redis
         version: v1
         metadata:
         - name: redisHost
           value: ""{1}""
         - name: redisPassword
           value: ""{2}""";

    public async Task InitializeAsync()
    {
        _network = new NetworkBuilder()
            .WithName(Guid.NewGuid().ToString("D"))
            .Build();
        await _network.CreateAsync().ConfigureAwait(false);

        var redisAlias = "redis";
        _redisContainer = new RedisBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases(redisAlias)
            .Build();
        await _redisContainer.StartAsync().ConfigureAwait(false);

        var redisComponent = string.Format(_redisComponent, _stateStoreName, $"{redisAlias}:{RedisBuilder.RedisPort}", "");

        _daprContainer = new DaprBuilder()
            .WithLogLevel("debug")
            .WithAppId("my-app")
            .WithNetwork(_network)
            .WithResourcesPath("/DaprComponents")
            .WithResourceMapping(Encoding.Default.GetBytes(redisComponent), "/DaprComponents/statestore.yaml")
            .DependsOn(_redisContainer)
            .Build();
        await _daprContainer.StartAsync().ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _daprContainer.DisposeAsync().ConfigureAwait(false);
        await _redisContainer.DisposeAsync().ConfigureAwait(false);
        await _network.DeleteAsync().ConfigureAwait(false);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task StateCanBeSet()
    {            
        // Given
        using var client = new DaprClientBuilder()
            .UseHttpEndpoint(_daprContainer.GetHttpAddress())
            .UseGrpcEndpoint(_daprContainer.GetGrpcAddress())
            .Build();

        // When
        var healthy = await client.CheckHealthAsync();
        var cts = new CancellationTokenSource();

        var key = Guid.NewGuid().ToString();
        var setValue = $"Chicken {key}";

        await client.SaveStateAsync(_stateStoreName, key, setValue, cancellationToken: new CancellationTokenSource(5000).Token);
        var getResult = await client.GetStateAsync<string>(_stateStoreName, key, cancellationToken: new CancellationTokenSource(5000).Token);

        // Then
        Assert.Equal(setValue, getResult);
    }
}