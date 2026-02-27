namespace Testcontainers.EventHubs;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class EventHubsContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public EventHubsContainer(EventHubsConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Event Hubs connection string.
    /// </summary>
    /// <returns>The Event Hubs connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Endpoint", new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(EventHubsBuilder.EventHubsPort)).ToString());
        properties.Add("DefaultEndpointsProtocol", Uri.UriSchemeHttp);
        properties.Add("SharedAccessKeyName", "RootManageSharedAccessKey");
        properties.Add("SharedAccessKey", "SAS_KEY_VALUE");
        properties.Add("UseDevelopmentEmulator", "true");
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}