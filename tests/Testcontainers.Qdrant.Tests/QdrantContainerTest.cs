namespace Testcontainers.Qdrant;

public sealed class QdrantContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseQdrantContainer]
    private readonly QdrantContainer _qdrant = new QdrantBuilder().Build();

    public Task InitializeAsync()
    {
        return _qdrant.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _qdrant.DisposeAsync().AsTask();
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthReturnsValidResponse()
    {
        var uri = new Uri(_qdrant.GetGrpcConnectionString());
        var client = new QdrantClient(uri.Host, uri.Port);

        var response = await client.HealthAsync();
        Assert.NotEmpty(response.Title);
    }
    // # --8<-- [end:UseQdrantContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingReturnsValidResponse()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_qdrant.GetHttpConnectionString()),
        };

        var response = await client.GetAsync("/");
        Assert.True(response.IsSuccessStatusCode);
    }
}