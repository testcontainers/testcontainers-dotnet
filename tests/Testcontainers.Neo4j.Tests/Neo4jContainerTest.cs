namespace Testcontainers.Neo4j;

public sealed class Neo4jContainerTest : IAsyncLifetime
{
    private readonly Neo4jContainer _neo4jContainer = new Neo4jBuilder().Build();

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
    public void SessionReturnsDatabase()
    {
        // Given
        const string database = "neo4j";

        using var driver = GraphDatabase.Driver(_neo4jContainer.GetConnectionString());

        // When
        using var session = driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase(database));

        // Then
        Assert.Equal(database, session.SessionConfig.Database);
    }
}