namespace Testcontainers.Redpanda;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class RedpandaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public RedpandaContainer(RedpandaConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
    
    /// <summary>
    /// Gets the Redpanda connection string.
    /// </summary>
    /// <returns>The Redpanda connection string.</returns>
    public string GetBootstrapServers()
    {
        return string.Format("PLAINTEXT://{0}:{1}", Hostname, GetMappedPublicPort(RedpandaBuilder.Port));
    }
    
    /// <summary>
    /// Gets the Redpanda's Schema Registry connection string.
    /// </summary>
    /// <returns>The Redpanda's Schema Registry connection string.</returns>
    public string GetSchemaRegistryAddress()
    {
        var endpoint = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(RedpandaBuilder.SchemaRegistryPort));
        return endpoint.ToString();
    }

}