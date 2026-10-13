namespace Testcontainers.MongoDbAtlasLocal;

public abstract class MongoDbAtlasLocalContainerTest : IAsyncLifetime
{
    private readonly MongoDbAtlasLocalContainer _mongoDbAtlasLocalContainer;

    private MongoDbAtlasLocalContainerTest(MongoDbAtlasLocalContainer mongoDbAtlasLocalContainer)
    {
        _mongoDbAtlasLocalContainer = mongoDbAtlasLocalContainer;
    }

    // # --8<-- [start:UseMongoDbAtlasLocalContainer]
    public async ValueTask InitializeAsync()
    {
        await _mongoDbAtlasLocalContainer.StartAsync()
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
    public void ConnectionStateReturnsOpen()
    {
        // Given
        var client = new MongoClient(_mongoDbAtlasLocalContainer.GetConnectionString());

        // When
        using var databases = client.ListDatabases(TestContext.Current.CancellationToken);

        // Then
        Assert.Contains(databases.ToEnumerable(TestContext.Current.CancellationToken), database => database.TryGetValue("name", out var name) && "admin".Equals(name.AsString));
        Assert.Equal(_mongoDbAtlasLocalContainer.GetConnectionString(), _mongoDbAtlasLocalContainer.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task AtlasSearchReturnsMatchingDocuments()
    {
        // Given
        const string indexName = "default";

        var client = new MongoClient(_mongoDbAtlasLocalContainer.GetConnectionString());

        var collection = client.GetDatabase("test").GetCollection<BsonDocument>("movies");

        await collection.InsertManyAsync(new[] { new BsonDocument("title", "The Matrix"), new BsonDocument("title", "Back to the Future"), new BsonDocument("title", "The Matrix Reloaded") }, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await collection.SearchIndexes.CreateOneAsync(new CreateSearchIndexModel(indexName, new BsonDocument("mappings", new BsonDocument("dynamic", true))), TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        await AtlasSearch.WaitUntilSearchIndexIsQueryableAsync(collection, indexName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var titles = await AtlasSearch.WaitUntilSearchReturnsAsync(collection, indexName, Builders<BsonDocument>.Search.Text("title", "matrix"), 2, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(new[] { "The Matrix", "The Matrix Reloaded" }, titles.OrderBy(title => title));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "printjson(db.adminCommand({listDatabases:1,nameOnly:true,filter:{\"name\":/^admin/}}));";

        // When
        var execResult = await _mongoDbAtlasLocalContainer.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UseMongoDbAtlasLocalContainer]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _mongoDbAtlasLocalContainer.DisposeAsync();
    }

    // # --8<-- [start:CreateMongoDbAtlasLocalContainer]
    [UsedImplicitly]
    public sealed class MongoDbAtlasLocalDefaultConfiguration : MongoDbAtlasLocalContainerTest
    {
        public MongoDbAtlasLocalDefaultConfiguration()
            : base(new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile()).Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class MongoDbAtlasLocalAuthConfiguration : MongoDbAtlasLocalContainerTest
    {
        public MongoDbAtlasLocalAuthConfiguration()
            : base(new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile()).WithUsername("mongo").WithPassword("mongo").Build())
        {
        }
    }
    // # --8<-- [end:CreateMongoDbAtlasLocalContainer]
}