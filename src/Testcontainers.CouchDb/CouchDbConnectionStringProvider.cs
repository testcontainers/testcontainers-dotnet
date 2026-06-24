namespace Testcontainers.CouchDb;

/// <summary>
/// Provides the CouchDb connection string.
/// </summary>
internal sealed class CouchDbConnectionStringProvider : ContainerConnectionStringProvider<CouchDbContainer, CouchDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}