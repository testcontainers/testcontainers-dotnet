namespace Testcontainers.Qdrant;

public sealed class QdrantContainerTest : IAsyncLifetime
{
    private readonly QdrantContainer _qdrantContainer = new QdrantBuilder().Build();

    public Task InitializeAsync()
    {
        return _qdrantContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _qdrantContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingReturnsValidResponse()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString()),
        };

        var response = await client.GetAsync("/");
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListCollectionsReturnsValidResponse()
    {
        var uri = new Uri(_qdrantContainer.GetGrpcConnectionString());
        var client = new QdrantClient(uri.Host, uri.Port);

        var response = await client.ListCollectionsAsync();
        Assert.Empty(response);
    }
}