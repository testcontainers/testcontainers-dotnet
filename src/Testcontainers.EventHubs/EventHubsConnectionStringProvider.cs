namespace Testcontainers.EventHubs;

/// <summary>
/// Provides the Event Hubs connection string.
/// </summary>
internal sealed class EventHubsConnectionStringProvider : ContainerConnectionStringProvider<EventHubsContainer, EventHubsConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}