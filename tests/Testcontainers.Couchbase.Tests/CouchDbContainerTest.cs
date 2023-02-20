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
    public void Test()
    {
    }
}