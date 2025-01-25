using System.Threading;

namespace Testcontainers.EventHubs;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class EventHubsContainer : DockerContainer
{
    private const string SharedAccessKeyName = "RootManageSharedAccessKey";

    private const string SharedAccessKey = "SAS_KEY_VALUE";

    private const string UseDevelopmentEmulator = "true";
    
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
    /// Gets the event hub connection string.
    /// </summary>
    /// <returns>The event hub connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Endpoint", new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(EventHubsBuilder.EventHubsPort)).ToString());
        properties.Add("DefaultEndpointsProtocol", Uri.UriSchemeHttp);
        properties.Add("SharedAccessKeyName", SharedAccessKeyName);
        properties.Add("SharedAccessKey", SharedAccessKey);
        properties.Add("UseDevelopmentEmulator", UseDevelopmentEmulator);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    protected override async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
        await _configuration.Networks.Single().CreateAsync(ct)
            .ConfigureAwait(false);

        await base.UnsafeCreateAsync(ct)
            .ConfigureAwait(false);
    }

    protected override Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
        return Task.WhenAll(base.UnsafeDeleteAsync(ct), _configuration.AzuriteContainer.DisposeAsync().AsTask());
    }

    protected override async Task UnsafeStartAsync(CancellationToken ct = default)
    {
        await _configuration.AzuriteContainer.StartAsync(ct)
            .ConfigureAwait(false);

        await base.UnsafeStartAsync(ct)
            .ConfigureAwait(false);
    }

    protected override async Task UnsafeStopAsync(CancellationToken ct = default)
    {
        await base.UnsafeStopAsync(ct)
            .ConfigureAwait(false);

        await _configuration.AzuriteContainer.StopAsync(ct)
            .ConfigureAwait(false);
    }
}