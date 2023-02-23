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
    /// Gets the Schema Registry address.
    /// </summary>
    /// <returns>The Schema Registry address.</returns>
    public string GetSchemaRegistryAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(RedpandaBuilder.SchemaRegistryPort)).ToString();
    }

    /// <summary>
    /// Gets the broker address.
    /// </summary>
    /// <returns>The broker address.</returns>
    public string GetBrokerAddress()
    {
        return new UriBuilder("PLAINTEXT", Hostname, GetMappedPublicPort(RedpandaBuilder.BrokerPort)).ToString();
    }
}