using System.Collections.Generic;

namespace Testcontainers.Kafka;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class KafkaContainer : DockerContainer
{
    private KafkaConfiguration _configuration;
    internal IEnumerable<string> AdvertisedListeners => this._configuration.AdvertisedListeners;
    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public KafkaContainer(KafkaConfiguration configuration)
        : base(configuration)
    {
        this._configuration = configuration;
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