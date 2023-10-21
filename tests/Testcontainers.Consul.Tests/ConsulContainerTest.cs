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
    public async Task GetItemReturnsPutItem()
    {
        // Given
        const string helloWorld = "Hello, World!";

        var key = Guid.NewGuid().ToString("D");

        var expected = new KVPair(key);
        expected.Value = Encoding.Default.GetBytes(helloWorld);

        var consulClientConfiguration = new ConsulClientConfiguration();
        consulClientConfiguration.Address = new Uri(_consulContainer.GetBaseAddress());

        using var consulClient = new ConsulClient(consulClientConfiguration);

        // When
        _ = await consulClient.KV.Put(expected)
            .ConfigureAwait(false);

        var actual = await consulClient.KV.Get(key)
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(helloWorld, Encoding.Default.GetString(actual.Response.Value));
    }
}