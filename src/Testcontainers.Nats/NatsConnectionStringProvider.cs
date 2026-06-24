namespace Testcontainers.Nats;

/// <summary>
/// Provides the NATS connection string.
/// </summary>
internal sealed class NatsConnectionStringProvider : ContainerConnectionStringProvider<NatsContainer, NatsConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}