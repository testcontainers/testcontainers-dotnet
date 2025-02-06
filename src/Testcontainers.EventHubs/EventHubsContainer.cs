namespace Testcontainers.EventHubs;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class EventHubsContainer : DockerContainer
{
    private readonly EventHubsConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public EventHubsContainer(EventHubsConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Event Hubs connection string.
    /// </summary>
    /// <returns>The Event Hubs connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>
        {
            {
                "Endpoint",
                new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(EventHubsBuilder.EventHubsPort))
                    .ToString()
            },
            { "DefaultEndpointsProtocol", Uri.UriSchemeHttp },
            { "SharedAccessKeyName", "RootManageSharedAccessKey" },
            { "SharedAccessKey", "SAS_KEY_VALUE" },
            { "UseDevelopmentEmulator", "true" },
        };
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }
}