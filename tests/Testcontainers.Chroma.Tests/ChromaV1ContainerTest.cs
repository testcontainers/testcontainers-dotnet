namespace Testcontainers.Chroma;

// Chroma 0.5.15 is the last release with only the v1 API: the container must be ready
// although the heartbeat of the v2 API does not answer.
public sealed class ChromaV1ContainerTest : IAsyncLifetime
{
    private readonly ChromaContainer _chromaContainer = new ChromaBuilder(TestSession.GetImageFromDockerfile(stage: "v1")).Build();

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
    public async Task GetV1HeartbeatReturnsHttpStatusCodeOk()
    {
        // Given
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(_chromaContainer.GetBaseAddress());

        // When
        using var httpResponse = await httpClient.GetAsync("/api/v1/heartbeat", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
    }
}
