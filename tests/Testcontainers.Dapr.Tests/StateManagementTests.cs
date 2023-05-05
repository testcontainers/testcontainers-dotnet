using System.Text;
using System.Threading;
using Testcontainers.Redis;

namespace Testcontainers.Dapr;

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