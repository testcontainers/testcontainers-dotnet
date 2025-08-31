namespace Testcontainers.PubSub;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PubSubContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public PubSubContainer(PubSubConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the PubSub emulator endpoint.
    /// </summary>
    /// <returns>The PubSub emulator endpoint.</returns>
    public string GetEmulatorEndpoint()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(PubSubBuilder.PubSubPort)
        ).ToString();
    }
}
