namespace Testcontainers.PubSub;

/// <summary>
/// Provides the PubSub connection string.
/// </summary>
internal sealed class PubSubConnectionStringProvider : ContainerConnectionStringProvider<PubSubContainer, PubSubConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEmulatorEndpoint();
    }
}