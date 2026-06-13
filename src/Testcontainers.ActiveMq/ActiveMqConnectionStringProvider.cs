namespace Testcontainers.ActiveMq;

/// <summary>
/// Provides the ActiveMq connection string.
/// </summary>
internal sealed class ActiveMqConnectionStringProvider : ContainerConnectionStringProvider<ArtemisContainer, ActiveMqConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBrokerAddress();
    }
}