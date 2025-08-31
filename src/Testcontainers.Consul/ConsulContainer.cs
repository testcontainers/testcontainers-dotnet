namespace Testcontainers.Consul;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ConsulContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ConsulContainer(ConsulConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the Consul base address.
    /// </summary>
    /// <returns>The Consul base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(ConsulBuilder.ConsulHttpPort)
        ).ToString();
    }
}
