namespace Testcontainers.Qdrant;

public sealed class QdrantDefaultContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseQdrantContainer]
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
    public async Task HealthReturnsValidResponse()
    {
        // Given
        using var client = new QdrantClient(new Uri(_qdrantContainer.GetGrpcConnectionString()));

        // When
        var response = await client.HealthAsync()
            .ConfigureAwait(true);

        // Then
        Assert.NotEmpty(response.Title);
    }
    // # --8<-- [end:UseQdrantContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetRootEndpointReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString());

        // When
        using var httpResponse = await httpClient.GetAsync("/")
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }
}