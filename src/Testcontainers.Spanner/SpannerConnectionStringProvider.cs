namespace Testcontainers.Spanner;

/// <summary>
/// Provides the Spanner connection string.
/// </summary>
internal sealed class SpannerConnectionStringProvider : ContainerConnectionStringProvider<SpannerContainer, SpannerConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEmulatorGrpcEndpoint();
    }
}