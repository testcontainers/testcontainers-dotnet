namespace Testcontainers.Couchbase;

public sealed class CouchbaseContainerTest : IAsyncLifetime
{
    private readonly CouchbaseContainer _couchbaseContainer = new CouchbaseBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _couchbaseContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _couchbaseContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetBucketReturnsDefaultBucket()
    {
        // Given
        var clusterOptions = new ClusterOptions();
        clusterOptions.ConnectionString = _couchbaseContainer.GetConnectionString();
        clusterOptions.UserName = CouchbaseBuilder.DefaultUsername;
        clusterOptions.Password = CouchbaseBuilder.DefaultPassword;

        var cluster = await Cluster.ConnectAsync(clusterOptions)
            .ConfigureAwait(true);

        // When
        var ping = await cluster.PingAsync()
            .ConfigureAwait(true);

        var bucket = await cluster.BucketAsync(_couchbaseContainer.Buckets.Single().Name)
            .ConfigureAwait(true);

        // Then
        Assert.NotEmpty(ping.Id);
        Assert.NotEmpty(ping.Services);
        Assert.NotEmpty(bucket.Name);
    }
}