namespace Testcontainers.Redpanda;

/// <summary>
/// Provides the Redpanda connection string.
/// </summary>
internal sealed class RedpandaConnectionStringProvider : ContainerConnectionStringProvider<RedpandaContainer, RedpandaConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBootstrapAddress();
    }
}