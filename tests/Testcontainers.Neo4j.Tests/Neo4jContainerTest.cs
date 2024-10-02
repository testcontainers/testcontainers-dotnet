namespace Testcontainers.Neo4j;

public abstract class Neo4jContainerTest : IAsyncLifetime
{
    private const string Neo4jDatabase = "neo4j";

    // # --8<-- [start:UseNeo4jContainer]
    private readonly Neo4jContainer _neo4jContainer;

    private Neo4jContainerTest(Neo4jContainer neo4jContainer)
    {
        _neo4jContainer = neo4jContainer;
    }

    public Task InitializeAsync()
    {
        return _neo4jContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _neo4jContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task SessionReturnsDatabase()
    {
        // Given
        await using var driver = GraphDatabase.Driver(_neo4jContainer.GetConnectionString());

        // When
        await using var session = driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase(Neo4jDatabase));

        // Then
        Assert.Equal(Neo4jDatabase, session.SessionConfig.Database);
    }
    // # --8<-- [end:UseNeo4jContainer]

    [UsedImplicitly]
    public sealed class Neo4jDefaultConfiguration : Neo4jContainerTest
    {
        public Neo4jDefaultConfiguration()
            : base(new Neo4jBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class Neo4jEnterpriseConfiguration : Neo4jContainerTest
    {
        public Neo4jEnterpriseConfiguration()
            : base(new Neo4jBuilder().WithEnterpriseEdition(true).Build())
        {
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task DatabaseShouldReturnEnterpriseEdition()
        {
            // Given
            await using var driver = GraphDatabase.Driver(_neo4jContainer.GetConnectionString());

            // When
            await using var session = driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase(Neo4jDatabase));
            var result = await session.RunAsync("CALL dbms.components() YIELD edition RETURN edition");
            var record = await result.SingleAsync();
            var edition = record["edition"].As<string>();

            // Then
            Assert.Equal("enterprise", edition);
        }
    }
}