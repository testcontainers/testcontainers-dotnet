namespace Testcontainers.MongoDb;

public abstract class MongoDbContainerTest : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer;
    private readonly bool _isReplicaSet;

    private MongoDbContainerTest(MongoDbContainer mongoDbContainer, bool isReplicaSet = false)
    {
        _mongoDbContainer = mongoDbContainer;
        _isReplicaSet = isReplicaSet;
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

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ReplicaSetStatus()
    {
        // Given
        const string scriptContent = "rs.status().ok;";

        // When
        var execResult = await _mongoDbContainer.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        if (_isReplicaSet)
        {
            Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
            Assert.Empty(execResult.Stderr);
            Assert.Equal("true\n", execResult.Stdout);
        }
        else
        {
            Assert.Equal(1L, execResult.ExitCode);
            Assert.Equal("MongoServerError: not running with --replSet\n", execResult.Stderr);
        }
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
            : base(new MongoDbBuilder().WithImage("mongo:4.4").Build(), isReplicaSet: true) // Replica set status returns ok in MongoDB 4.4 without initialising it
        {
        }
    }

    [UsedImplicitly]
    public sealed class MongoDbReplicaSetDefaultConfiguration : MongoDbContainerTest
    {
        public MongoDbReplicaSetDefaultConfiguration()
            : base(new MongoDbBuilder().WithReplicaSet().Build(), isReplicaSet: true)
        {
        }
    }

    [UsedImplicitly]
    public sealed class MongoDbNamedReplicaSetConfiguration : MongoDbContainerTest
    {
        public MongoDbNamedReplicaSetConfiguration()
            : base(new MongoDbBuilder().WithReplicaSet("rscustom").Build(), isReplicaSet: true)
        {
        }
    }
}