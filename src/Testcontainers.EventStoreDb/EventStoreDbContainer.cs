namespace Testcontainers.EventStoreDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class EventStoreDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public EventStoreDbContainer(EventStoreDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the EventStoreDb connection string.
    /// </summary>
    /// <returns>The EventStoreDb connection string.</returns>
    public string GetConnectionString()
    {
        var endpoint = new UriBuilder("esdb", Hostname, GetMappedPublicPort(EventStoreDbBuilder.EventStoreDbPort));
        endpoint.Query = "tls=false";
        return endpoint.ToString();
    }
}