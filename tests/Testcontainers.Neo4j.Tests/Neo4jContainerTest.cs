namespace Testcontainers.Neo4j;

public sealed class Neo4jContainerTest : IAsyncLifetime
{
    private readonly Neo4jContainer _neo4JContainer = new Neo4jBuilder().Build();

    public Task InitializeAsync()
    {
        return _neo4JContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _neo4JContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void SessionReturnsDatabase()
    {
        // Given
        const string database = "neo4j";

        using var driver = GraphDatabase.Driver(_neo4JContainer.GetConnectionString());

        // When
        using var session = driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));

        // Then
        Assert.Equal(database, session.SessionConfig.Database);
    }
}