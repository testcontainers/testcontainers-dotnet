namespace Testcontainers.Couchbase;

public sealed class CouchbaseContainerTest : IAsyncLifetime
{
    private readonly CouchbaseContainer _couchbaseContainer = new CouchbaseBuilder().Build();

    public Task InitializeAsync()
    {
        return _couchbaseContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _couchbaseContainer.DisposeAsync().AsTask();
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
            .ConfigureAwait(false);

        // When
        var ping = await cluster.PingAsync()
            .ConfigureAwait(false);

        var bucket = await cluster.BucketAsync(_couchbaseContainer.Buckets.Single().Name)
            .ConfigureAwait(false);

        // Then
        Assert.NotEmpty(ping.Id);
        Assert.NotEmpty(ping.Services);
        Assert.NotEmpty(bucket.Name);
    }
}