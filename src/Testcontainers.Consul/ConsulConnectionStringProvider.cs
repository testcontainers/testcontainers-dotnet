namespace Testcontainers.Consul;

/// <summary>
/// Provides the Consul connection string.
/// </summary>
internal sealed class ConsulConnectionStringProvider : ContainerConnectionStringProvider<ConsulContainer, ConsulConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}