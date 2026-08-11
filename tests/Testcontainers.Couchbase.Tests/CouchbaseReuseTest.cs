namespace Testcontainers.Couchbase;

public sealed class CouchbaseReuseTest : IAsyncLifetime
{
    private readonly string _labelKey = Guid.NewGuid().ToString("D");

    private readonly string _labelValue = Guid.NewGuid().ToString("D");

    private readonly IList<CouchbaseContainer> _containers = new List<CouchbaseContainer>();

    public async ValueTask InitializeAsync()
    {
        for (var _ = 0; _ < 3; _++)
        {
            var container = new CouchbaseBuilder(TestSession.GetImageFromDockerfile())
                .WithLabel(_labelKey, _labelValue)
                .WithReuse(true)
                .Build();

            await container.StartAsync()
                .ConfigureAwait(false);

            _containers.Add(container);
        }
    }

    public async ValueTask DisposeAsync()
    {
        await Task.WhenAll(_containers
            .Take(1)
            .Select(container =>
            {
                // We do not want to leak resources, but `WithCleanUp(true)` cannot be used
                // alongside `WithReuse(true)`. As a workaround, we set the `SessionId` using
                // reflection afterward to delete the container.
                container.AsDynamic()._configuration.SessionId = ResourceReaper.DefaultSessionId;
                return container.DisposeAsync().AsTask();
            }));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReusesSameContainerAcrossMultipleStarts()
    {
        // Given
        var couchbaseContainer = _containers[^1];

        var clusterOptions = new ClusterOptions();
        clusterOptions.ConnectionString = couchbaseContainer.GetConnectionString();
        clusterOptions.UserName = CouchbaseBuilder.DefaultUsername;
        clusterOptions.Password = CouchbaseBuilder.DefaultPassword;

        // When
        var cluster = await Cluster.ConnectAsync(clusterOptions, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var ping = await cluster.PingAsync()
            .ConfigureAwait(true);

        // Then
        Assert.NotEmpty(ping.Services);
        Assert.Single(_containers.Select(container => container.Id).Distinct());
    }
}
