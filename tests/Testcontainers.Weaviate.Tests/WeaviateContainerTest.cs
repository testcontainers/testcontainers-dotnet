namespace Testcontainers.Weaviate;

public sealed class WeaviateContainerTest : IAsyncLifetime
{
    private readonly WeaviateContainer _weaviateContainer = new WeaviateBuilder().Build();

    public Task InitializeAsync() => _weaviateContainer.StartAsync();

    public Task DisposeAsync() => _weaviateContainer.DisposeAsync().AsTask();

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetSchemaReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_weaviateContainer.GetBaseAddress());

        // When
        using var httpResponse = await httpClient.GetAsync("v1/schema")
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }
}