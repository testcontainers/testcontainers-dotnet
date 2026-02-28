namespace Testcontainers.ClickHouse;

/// <summary>
/// Provides the ClickHouse connection string.
/// </summary>
internal sealed class ClickHouseConnectionStringProvider : ContainerConnectionStringProvider<ClickHouseContainer, ClickHouseConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}