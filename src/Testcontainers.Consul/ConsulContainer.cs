namespace Testcontainers.Consul;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ConsulContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsulContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public ConsulContainer(ConsulConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the Consul connection string.
    /// </summary>
    /// <returns>The Consul connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ConsulBuilder.ConsulHttpPort)).ToString();
    }
}