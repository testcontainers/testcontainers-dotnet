namespace Testcontainers.Toxiproxy;

public sealed class ToxiproxyContainerTest : IAsyncLifetime
{
    private const string RedisNetworkAlias = "redis-container";

    // # --8<-- [start:CreateToxiproxyContainer]
    private readonly INetwork _network = new NetworkBuilder().Build();

    private readonly IContainer _redisContainer;

    private readonly IContainer _toxiproxyContainer;

    public ToxiproxyContainerTest()
    {
        _redisContainer = new RedisBuilder()
            .WithNetwork(_network)
            .WithNetworkAliases(RedisNetworkAlias)
            .Build();

        _toxiproxyContainer = new ToxiproxyBuilder()
            .WithNetwork(_network)
            .Build();
    }

    public async ValueTask InitializeAsync()
    {
        await _redisContainer.StartAsync()
            .ConfigureAwait(false);

        await _toxiproxyContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _toxiproxyContainer.DisposeAsync()
            .ConfigureAwait(false);

        await _redisContainer.DisposeAsync()
            .ConfigureAwait(false);

        await _network.DisposeAsync()
            .ConfigureAwait(false);
    }
    // # --8<-- [end:CreateToxiproxyContainer]

    // # --8<-- [start:UseToxiproxyContainer]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task LatencyToxicIncreasesResponseTime()
    {
        // Given

        // The Toxiproxy module initializes 32 ports during startup that we can
        // use to configure the proxy and redirect traffic.
        const ushort toxiproxyPort = ToxiproxyBuilder.FirstProxiedPort;

        const string key = "key";

        const string value = "value";

        // GetMappedPublicPort() retrieves the first bound port, which corresponds
        // to Toxiproxy's control port.
        using var connection = new Connection(_toxiproxyContainer.Hostname, _toxiproxyContainer.GetMappedPublicPort());
        var client = connection.Client();

        // The proxy configuration forwards traffic from the test host to the
        // Toxiproxy container and then to the Redis container.

        // # --8<-- [start:ProxyConfiguration]
        var proxy = new Proxy();
        proxy.Name = "redis";
        proxy.Enabled = true;
        proxy.Listen = "0.0.0.0:" + toxiproxyPort;
        proxy.Upstream = RedisNetworkAlias + ":6379";
        var redisProxy = client.Add(proxy);
        // # --8<-- [end:ProxyConfiguration]

        // Don't establish a connection directly to the Redis container.
        // Instead, connect through the Toxiproxy container using the first of
        // the 32 ports initialized during the Toxiproxy module startup.

        // # --8<-- [start:ConnectThroughToxiproxy]
        var connectionString = new UriBuilder("redis", _toxiproxyContainer.Hostname, _toxiproxyContainer.GetMappedPublicPort(toxiproxyPort)).Uri.Authority;
        // # --8<-- [end:ConnectThroughToxiproxy]

        using var redis = await ConnectionMultiplexer.ConnectAsync(connectionString)
            .ConfigureAwait(true);

        var db = redis.GetDatabase();

        await db.StringSetAsync(key, value)
            .ConfigureAwait(true);

        // When
        var latencyWithoutToxic = await MeasureLatencyAsync(db, key, value)
            .ConfigureAwait(true);

        // Apply the toxic to the proxy. In this case, add additional
        // latency to the downstream traffic.

        // # --8<-- [start:ToxicConfiguration]
        var latencyToxic = new LatencyToxic();
        latencyToxic.Name = "latency_downstream";
        latencyToxic.Stream = ToxicDirection.DownStream;
        latencyToxic.Toxicity = 1;
        latencyToxic.Attributes.Latency = 1100;
        latencyToxic.Attributes.Jitter = 100;
        redisProxy.Add(latencyToxic);
        // # --8<-- [end:ToxicConfiguration]

        var latencyWithToxic = await MeasureLatencyAsync(db, key, value)
            .ConfigureAwait(true);

        // Then
        Assert.InRange(latencyWithoutToxic, 0, 250);
        Assert.InRange(latencyWithToxic, 1000, 1500);
    }
    // # --8<-- [end:UseToxiproxyContainer]

    private static async Task<double> MeasureLatencyAsync(IDatabase db, string key, string expectedValue)
    {
        var stopwatch = Stopwatch.StartNew();

        var value = await db.StringGetAsync(key)
            .ConfigureAwait(true);

        stopwatch.Stop();

        Assert.Equal(expectedValue, value);

        return stopwatch.Elapsed.TotalMilliseconds;
    }
}