namespace Testcontainers.Redpanda;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class RedpandaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public RedpandaContainer(RedpandaConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the Schema Registry address.
    /// </summary>
    /// <returns>The Schema Registry address.</returns>
    public string GetSchemaRegistryAddress()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(RedpandaBuilder.SchemaRegistryPort)
        ).ToString();
    }

    /// <summary>
    /// Gets the bootstrap address.
    /// </summary>
    /// <returns>The bootstrap address.</returns>
    public string GetBootstrapAddress()
    {
        return new UriBuilder(
            "PLAINTEXT",
            Hostname,
            GetMappedPublicPort(RedpandaBuilder.RedpandaPort)
        ).ToString();
    }
}
