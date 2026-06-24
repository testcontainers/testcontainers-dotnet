namespace Testcontainers.Pulsar;

/// <summary>
/// Provides the Pulsar connection string.
/// </summary>
internal sealed class PulsarConnectionStringProvider : ContainerConnectionStringProvider<PulsarContainer, PulsarConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBrokerAddress();
    }
}