﻿namespace Testcontainers.EventStore;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class EventStoreContainer : DockerContainer
{
    private readonly EventStoreConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public EventStoreContainer(EventStoreConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the EventStore connection string.
    /// </summary>
    /// <returns>The EventStore connection string.</returns>
    public string GetConnectionString() =>
        $"esdb://{Hostname}:{GetMappedPublicPort(EventStoreBuilder.EventStorePort)}?tls=false";
}