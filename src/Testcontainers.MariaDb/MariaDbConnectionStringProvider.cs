namespace Testcontainers.MariaDb;

/// <summary>
/// Provides the MariaDb connection string.
/// </summary>
internal sealed class MariaDbConnectionStringProvider : ContainerConnectionStringProvider<MariaDbContainer, MariaDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}