namespace Testcontainers.Neo4j;

/// <summary>
/// Provides the Neo4j connection string.
/// </summary>
internal sealed class Neo4jConnectionStringProvider : ContainerConnectionStringProvider<Neo4jContainer, Neo4jConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}