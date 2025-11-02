namespace Testcontainers.ServiceBus;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ServiceBusContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ServiceBusContainer(ServiceBusConfiguration configuration)
        : base(configuration)
    {
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
}