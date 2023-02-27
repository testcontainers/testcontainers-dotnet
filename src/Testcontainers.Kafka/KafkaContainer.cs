namespace Testcontainers.Kafka;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class KafkaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public KafkaContainer(KafkaConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the broker address.
    /// </summary>
    /// <returns>The broker address.</returns>
    public string GetBootstrapAddress()
    {
        return new UriBuilder("PLAINTEXT", Hostname, GetMappedPublicPort(KafkaBuilder.KafkaPort)).ToString();
    }
}