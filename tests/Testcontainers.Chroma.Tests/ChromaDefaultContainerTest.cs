namespace Testcontainers.Chroma;

public sealed class ChromaDefaultContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseChromaContainer]
    private readonly ChromaContainer _chromaContainer = new ChromaBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _chromaContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _chromaContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task QueryReturnsNearestRecord()
    {
        // Given
        using var client = new ChromaClient(_chromaContainer.GetConnectionString());

        var collection = await client.CreateCollectionAsync("documents", cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var collectionClient = client.GetCollectionClient(collection);

        await collectionClient.AddAsync(["a", "b"], [new[] { 1f, 0f }, new[] { 0f, 1f }], cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        var results = await collectionClient.QueryAsync(new[] { 0.9f, 0.1f }, nResults: 1, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal("a", Assert.Single(results).Id);
    }
    // # --8<-- [end:UseChromaContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetHeartbeatReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_chromaContainer.GetBaseAddress());

        // When
        using var httpResponse = await httpClient.GetAsync("/api/v2/heartbeat", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.Equal(_chromaContainer.GetBaseAddress(), _chromaContainer.GetConnectionString());
    }
}
