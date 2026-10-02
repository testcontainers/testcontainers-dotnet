namespace Testcontainers.MongoDbAtlasLocal;

public sealed class MongoDbAtlasLocalSeedTest : IAsyncLifetime
{
    private const string Database = "sample";

    private const string SeedMarkerFilePath = "/tmp/seeded";

    // # --8<-- [start:SeedMongoDbAtlasLocalContainer]
    private readonly MongoDbAtlasLocalContainer _mongoDbAtlasLocalContainer = new MongoDbAtlasLocalBuilder(TestSession.GetImageFromDockerfile())
        .WithUsername("mongo")
        .WithPassword("mongo")
        .WithInitDatabase(Database)
        .WithInitScript("Seed/01-movies.js")
        .WithInitScriptContent("02-marker.sh", "touch " + SeedMarkerFilePath)
        .WithNoTelemetry()
        .Build();
    // # --8<-- [end:SeedMongoDbAtlasLocalContainer]

    public async ValueTask InitializeAsync()
    {
        await _mongoDbAtlasLocalContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _mongoDbAtlasLocalContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SeededSearchIndexReturnsMatchingDocuments()
    {
        // Given
        const string indexName = "default";

        var client = new MongoClient(_mongoDbAtlasLocalContainer.GetConnectionString());

        var collection = client.GetDatabase(Database).GetCollection<BsonDocument>("movies");

        // When
        var count = await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await AtlasSearch.WaitUntilSearchIndexIsQueryableAsync(collection, indexName, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var titles = await AtlasSearch.WaitUntilSearchReturnsAsync(collection, indexName, Builders<BsonDocument>.Search.Text("title", "matrix"), 2, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(3, count);
        Assert.Equal(new[] { "The Matrix", "The Matrix Reloaded" }, titles.OrderBy(title => title));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void UnauthenticatedClientIsRejected()
    {
        // Given
        var connectionString = new MongoUrlBuilder(_mongoDbAtlasLocalContainer.GetConnectionString()) { Username = null, Password = null }.ToString();

        var client = new MongoClient(connectionString);

        // When
        var exception = Assert.Throws<MongoCommandException>(() => client.ListDatabaseNames(TestContext.Current.CancellationToken));

        // Then
        Assert.Equal("Unauthorized", exception.CodeName);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShellInitScriptRan()
    {
        // Given
        var command = new[] { "test", "-f", SeedMarkerFilePath };

        // When
        var execResult = await _mongoDbAtlasLocalContainer.ExecAsync(command, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(0L, execResult.ExitCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task TelemetryIsDisabled()
    {
        // Given
        var command = new[] { "printenv", "DO_NOT_TRACK" };

        // When
        var execResult = await _mongoDbAtlasLocalContainer.ExecAsync(command, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal("1", execResult.Stdout.Trim());
    }
}