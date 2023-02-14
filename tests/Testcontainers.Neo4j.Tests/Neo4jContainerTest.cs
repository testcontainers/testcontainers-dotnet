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
}