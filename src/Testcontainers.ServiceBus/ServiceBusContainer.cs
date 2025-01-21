namespace Testcontainers.ServiceBus;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ServiceBusContainer : DockerContainer
{
    private readonly ServiceBusConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ServiceBusContainer(ServiceBusConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Service Bus connection string.
    /// </summary>
    /// <returns>The Service Bus connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Endpoint", new UriBuilder("amqp", Hostname, GetMappedPublicPort(ServiceBusBuilder.ServiceBusPort)).ToString());
        properties.Add("SharedAccessKeyName", "RootManageSharedAccessKey");
        properties.Add("SharedAccessKey", "SAS_KEY_VALUE");
        properties.Add("UseDevelopmentEmulator", "true");
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <inheritdoc />
    protected override async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
        await _configuration.Networks.Single().CreateAsync(ct)
            .ConfigureAwait(false);

        await base.UnsafeCreateAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
        return Task.WhenAll(base.UnsafeDeleteAsync(ct), _configuration.DatabaseContainer.DisposeAsync().AsTask());
    }

    /// <inheritdoc />
    protected override async Task UnsafeStartAsync(CancellationToken ct = default)
    {
        await _configuration.DatabaseContainer.StartAsync(ct)
            .ConfigureAwait(false);

        await base.UnsafeStartAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeStopAsync(CancellationToken ct = default)
    {
        await base.UnsafeStopAsync(ct)
            .ConfigureAwait(false);

        await _configuration.DatabaseContainer.StopAsync(ct)
            .ConfigureAwait(false);
    }
}