namespace Testcontainers.Weaviate;

public sealed class WeaviateContainerTest : IAsyncLifetime
{
    private readonly WeaviateContainer _weaviateContainer = new WeaviateBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _weaviateContainer.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _weaviateContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetSchemaReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_weaviateContainer.GetBaseAddress());

        // When
        using var httpResponse = await httpClient
            .GetAsync("v1/schema", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }
}
