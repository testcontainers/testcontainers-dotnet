namespace Testcontainers.BigQuery;

/// <summary>
/// Provides the BigQuery connection string.
/// </summary>
internal sealed class BigQueryConnectionStringProvider : ContainerConnectionStringProvider<BigQueryContainer, BigQueryConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEmulatorEndpoint();
    }
}