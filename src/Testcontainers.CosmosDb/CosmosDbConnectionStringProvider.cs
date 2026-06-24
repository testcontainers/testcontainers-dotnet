namespace Testcontainers.CosmosDb;

/// <summary>
/// Provides the CosmosDb connection string.
/// </summary>
internal sealed class CosmosDbConnectionStringProvider : ContainerConnectionStringProvider<CosmosDbContainer, CosmosDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}