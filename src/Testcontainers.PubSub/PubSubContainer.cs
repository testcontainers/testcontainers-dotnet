namespace Testcontainers.PubSub;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PubSubContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public PubSubContainer(PubSubConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    public string GetEmulatorEndpoint() => new UriBuilder(Uri.UriSchemeHttp,Hostname,GetMappedPublicPort(PubSubBuilder.PubSubPort)).ToString();
}