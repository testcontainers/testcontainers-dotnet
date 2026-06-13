namespace Testcontainers.Kusto;

/// <summary>
/// Provides the Kusto connection string.
/// </summary>
internal sealed class KustoConnectionStringProvider : ContainerConnectionStringProvider<KustoContainer, KustoConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}