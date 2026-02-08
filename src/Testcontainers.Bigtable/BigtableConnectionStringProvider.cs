namespace Testcontainers.Bigtable;

/// <summary>
/// Provides the Bigtable connection string.
/// </summary>
internal sealed class BigtableConnectionStringProvider : ContainerConnectionStringProvider<BigtableContainer, BigtableConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEmulatorEndpoint();
    }
}