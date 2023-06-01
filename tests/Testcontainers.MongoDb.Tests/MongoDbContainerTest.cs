namespace Testcontainers.MongoDb;

public abstract class MongoDbContainerTest : ContainerTest<MongoDbBuilder, MongoDbContainer>
{
    protected MongoDbContainerTest(Action<MongoDbBuilder> configure = null) : base(configure)
    {
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        var client = new MongoClient(Container.GetConnectionString());

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
        var execResult = await Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(false);

        // When
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
    }

    [UsedImplicitly]
    public sealed class MongoDbDefaultConfiguration : MongoDbContainerTest
    {
    }

    [UsedImplicitly]
    public sealed class MongoDbV5Configuration : MongoDbContainerTest
    {
        public MongoDbV5Configuration()
            : base(builder => builder.WithImage("mongo:5.0"))
        {
        }
    }

    [UsedImplicitly]
    public sealed class MongoDbV4Configuration : MongoDbContainerTest
    {
        public MongoDbV4Configuration()
            : base(builder => builder.WithImage("mongo:4.4"))
        {
        }
    }
}