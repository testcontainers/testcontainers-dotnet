namespace Testcontainers.Grafana;

/// <summary>
/// Provides the Grafana connection string.
/// </summary>
internal sealed class GrafanaConnectionStringProvider : ContainerConnectionStringProvider<GrafanaContainer, GrafanaConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}