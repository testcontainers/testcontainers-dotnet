namespace Testcontainers.Consul;

public sealed class ConsulContainerTest : IAsyncLifetime
{
    private readonly ConsulContainer _consulContainer = new ConsulBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _consulContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _consulContainer.DisposeAsync();
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
        _ = await consulClient.KV.Put(expected, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var actual = await consulClient.KV.Get(key, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
        Assert.Equal(helloWorld, Encoding.Default.GetString(actual.Response.Value));
    }
}