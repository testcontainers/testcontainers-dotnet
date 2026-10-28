namespace Testcontainers.Chroma;

public abstract class ChromaContainerTest : IAsyncLifetime
{
    private readonly ChromaContainer _chromaContainer;

    private readonly ChromaApiVersion _apiVersion;

    private ChromaContainerTest(ChromaContainer chromaContainer, ChromaApiVersion apiVersion = ChromaApiVersion.V2)
    {
        _chromaContainer = chromaContainer;
        _apiVersion = apiVersion;
    }

    // # --8<-- [start:UseChromaContainer]
    public async ValueTask InitializeAsync()
    {
        await _chromaContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task QueryReturnsNearestRecord()
    {
        // Given
        var ids = new[] { "a", "b" };

        var embeddings = new ReadOnlyMemory<float>[] { new[] { 1f, 0f }, new[] { 0f, 1f } };

        var queryEmbedding = new[] { 0.9f, 0.1f };

        var options = new ChromaConfigurationOptions(_chromaContainer.GetBaseAddress()).WithApiVersion(_apiVersion);

        using var client = new ChromaClient(options);

        var collection = await client.CreateCollectionAsync("documents", cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var collectionClient = client.GetCollectionClient(collection);

        await collectionClient.AddAsync(ids, embeddings, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        var records = await collectionClient.QueryAsync(queryEmbedding, nResults: 1, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal("a", Assert.Single(records).Id);
        Assert.Equal(_chromaContainer.GetBaseAddress(), _chromaContainer.GetConnectionString());
    }
    // # --8<-- [end:UseChromaContainer]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _chromaContainer.DisposeAsync();
    }

    // # --8<-- [start:CreateChromaContainer]
    [UsedImplicitly]
    public sealed class ChromaDefaultConfiguration : ChromaContainerTest
    {
        public ChromaDefaultConfiguration()
            : base(new ChromaBuilder(TestSession.GetImageFromDockerfile()).Build())
        {
        }
    }
    // # --8<-- [end:CreateChromaContainer]

    [UsedImplicitly]
    public sealed class ChromaV0Configuration : ChromaContainerTest
    {
        public ChromaV0Configuration()
            : base(new ChromaBuilder(TestSession.GetImageFromDockerfile(stage: "v0_5_15")).Build(), ChromaApiVersion.V1 /* Chroma 0.5.15 is the last version without the v2 API. */)
        {
        }
    }
}