namespace Testcontainers.Bigtable;

/// <inheritdoc cref="DockerContainer"/>
[PublicAPI]
public sealed class BigtableContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BigtableContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public BigtableContainer(IContainerConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the Bigtable emulator endpoint.
    /// </summary>
    /// <returns>The Bigtable emulator endpoint.</returns>
    public string GetEmulatorEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(BigtableBuilder.BigtablePort)).ToString();
    }
}