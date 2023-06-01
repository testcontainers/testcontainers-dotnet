namespace Testcontainers.Neo4j;

public sealed class Neo4jContainerTest : ContainerTest<Neo4jBuilder, Neo4jContainer>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void SessionReturnsDatabase()
    {
        // Given
        const string database = "neo4j";

        using var driver = GraphDatabase.Driver(Container.GetConnectionString());

        // When
        using var session = driver.AsyncSession(sessionConfigBuilder => sessionConfigBuilder.WithDatabase("neo4j"));

        // Then
        Assert.Equal(database, session.SessionConfig.Database);
    }
}