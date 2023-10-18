using System.Net;
using System.Text;

namespace Testcontainers.Consul;

public sealed class ConsulContainerTest : IAsyncLifetime
{
    private readonly ConsulContainer _consulContainer = new ConsulBuilder().Build();

    public Task InitializeAsync()
    {
        return _consulContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _consulContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConnectionStateReturnsOpen()
    {
        using var consulClient = new ConsulClient(option => option.Address = new System.Uri(_consulContainer.GetConnectionString()));
        var putPair = new KVPair("hello")
        {
            Value = Encoding.UTF8.GetBytes("Hello Consul")
        };
        await consulClient.KV.Put(putPair);
        var result = await consulClient.KV.Get("hello");
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
}