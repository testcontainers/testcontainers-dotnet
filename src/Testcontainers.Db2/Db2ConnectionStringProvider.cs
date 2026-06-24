namespace Testcontainers.Db2;

/// <summary>
/// Provides the Db2 connection string.
/// </summary>
internal sealed class Db2ConnectionStringProvider : ContainerConnectionStringProvider<Db2Container, Db2Configuration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}