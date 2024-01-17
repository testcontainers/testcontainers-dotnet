namespace Testcontainers.MongoDb;

public abstract class MongoDbContainerTest : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer;

    private MongoDbContainerTest(MongoDbContainer mongoDbContainer)
    {
        _mongoDbContainer = mongoDbContainer;
    }

    public Task InitializeAsync()
    {
        return _mongoDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _mongoDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        var client = new MongoClient(_mongoDbContainer.GetConnectionString());

        // When
        using var databases = client.ListDatabases();

        // Then
        Assert.Contains(databases.ToEnumerable(), database => database.TryGetValue("name", out var name) && "admin".Equals(name.AsString));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "printjson(db.adminCommand({listDatabases:1,nameOnly:true,filter:{\"name\":/^admin/}}));";

        // When
        var execResult = await _mongoDbContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class MongoDbDefaultConfiguration : MongoDbContainerTest
    {
        public MongoDbDefaultConfiguration()
            : base(new MongoDbBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class MongoDbNoAuthConfiguration : MongoDbContainerTest
    {
        public MongoDbNoAuthConfiguration()
            : base(new MongoDbBuilder().WithUsername(string.Empty).WithPassword(string.Empty).Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class MongoDbV5Configuration : MongoDbContainerTest
    {
        public MongoDbV5Configuration()
            : base(new MongoDbBuilder().WithImage("mongo:5.0").Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class MongoDbV4Configuration : MongoDbContainerTest
    {
        public MongoDbV4Configuration()
            : base(new MongoDbBuilder().WithImage("mongo:4.4").Build())
        {
        }
    }
}