using System.Net.Http;
using System.Threading;

namespace Testcontainers.Dapr;

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