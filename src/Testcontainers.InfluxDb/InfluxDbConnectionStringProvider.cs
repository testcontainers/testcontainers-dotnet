namespace Testcontainers.InfluxDb;

/// <summary>
/// Provides the InfluxDb connection string.
/// </summary>
internal sealed class InfluxDbConnectionStringProvider : ContainerConnectionStringProvider<InfluxDbContainer, InfluxDbConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetAddress();
    }
}