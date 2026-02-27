namespace Testcontainers.ArangoDb;

/// <summary>
/// Provides the ArangoDb connection string.
/// </summary>
internal sealed class ArangoDbConnectionStringProvider : ContainerConnectionStringProvider<ArangoDbContainer, ArangoDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetTransportAddress();
    }
}