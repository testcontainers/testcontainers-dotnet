namespace Testcontainers.Pulsar;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PulsarContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PulsarContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public PulsarContainer(PulsarConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the service URL for the Pulsar cluster.
    /// </summary>
    /// <returns>The service URL.</returns>
    public Uri GetServiceUrl()
    {
        return new Uri($"pulsar://{Hostname}:{GetMappedPublicPort(PulsarBuilder.BrokerPort)}");
    }
}