namespace Testcontainers.Cassandra;

/// <summary>
/// Provides the Cassandra connection string.
/// </summary>
internal sealed class CassandraConnectionStringProvider : ContainerConnectionStringProvider<CassandraContainer, CassandraConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}