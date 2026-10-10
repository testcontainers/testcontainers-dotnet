namespace Testcontainers.FlociAz;

/// <summary>
/// Provides the FlociAz connection string.
/// </summary>
internal sealed class FlociAzConnectionStringProvider : ContainerConnectionStringProvider<FlociAzContainer, FlociAzConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}
