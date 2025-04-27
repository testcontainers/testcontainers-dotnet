namespace Testcontainers.Kafka;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class KafkaContainer : DockerContainer
{
    private readonly KafkaConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public KafkaContainer(KafkaConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the broker address.
    /// </summary>
    /// <returns>The broker address.</returns>
    public string GetBootstrapAddress()
    {
        return new UriBuilder("PLAINTEXT", Hostname, GetMappedPublicPort(KafkaBuilder.KafkaPort)).ToString();
    }

    /// <summary>
    /// Gets a list of advertised listeners.
    /// </summary>
    public IEnumerable<string> AdvertisedListeners
    {
        get
        {
            return _configuration.AdvertisedListeners;
        }
    }
}