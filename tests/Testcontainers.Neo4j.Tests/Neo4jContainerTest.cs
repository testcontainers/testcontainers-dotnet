namespace Testcontainers.Neo4j;

public abstract class Neo4jContainerTest : IAsyncLifetime
{
    private readonly Neo4jContainer _neo4jContainer;

    private Neo4jContainerTest(Neo4jContainer neo4jContainer)
    {
        _neo4jContainer = neo4jContainer;
    }

    public abstract string Edition { get; }

    // # --8<-- [start:UseNeo4jContainer]
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
        const string neo4jDatabase = "neo4j";

        using var driver = GraphDatabase.Driver(_neo4jContainer.GetConnectionString());

        // When
        using var session = driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase(neo4jDatabase));

        var result = await session.RunAsync("CALL dbms.components() YIELD edition RETURN edition")
            .ConfigureAwait(true);

        var record = await result.SingleAsync()
            .ConfigureAwait(true);

        var edition = record["edition"].As<string>();

        // Then
        Assert.Equal(neo4jDatabase, session.SessionConfig.Database);
        Assert.Equal(Edition, edition);
    }
    // # --8<-- [end:UseNeo4jContainer]

    // # --8<-- [start:CreateNeo4jContainer]
    [UsedImplicitly]
    public sealed class Neo4jDefaultConfiguration : Neo4jContainerTest
    {
        public Neo4jDefaultConfiguration()
            : base(new Neo4jBuilder().Build())
        {
        }

        public override string Edition => "community";
    }

    [UsedImplicitly]
    public sealed class Neo4jEnterpriseEditionConfiguration : Neo4jContainerTest
    {
        public Neo4jEnterpriseEditionConfiguration()
            : base(new Neo4jBuilder().WithEnterpriseEdition(true).Build())
        {
        }

        public override string Edition => "enterprise";
    }
    // # --8<-- [end:CreateNeo4jContainer]
}