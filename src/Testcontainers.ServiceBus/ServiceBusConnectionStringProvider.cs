namespace Testcontainers.ServiceBus;

/// <summary>
/// Provides the Service Bus connection string.
/// </summary>
internal sealed class ServiceBusConnectionStringProvider : ContainerConnectionStringProvider<ServiceBusContainer, ServiceBusConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}