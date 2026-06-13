namespace Testcontainers.Couchbase;

/// <summary>
/// Provides the Couchbase connection string.
/// </summary>
internal sealed class CouchbaseConnectionStringProvider : ContainerConnectionStringProvider<CouchbaseContainer, CouchbaseConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}