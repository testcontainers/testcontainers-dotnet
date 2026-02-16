namespace Testcontainers.Temporalio;

/// <summary>
/// Provides the Temporal connection string.
/// </summary>
internal sealed class TemporalConnectionStringProvider : ContainerConnectionStringProvider<TemporalContainer, TemporalConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetGrpcAddress();
    }
}