namespace Testcontainers.ArangoDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ArangoDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public ArangoDbContainer(ArangoDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the transport address.
    /// </summary>
    /// <returns>The transport address.</returns>
    public string GetTransportAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ArangoDbBuilder.ArangoDbPort)).ToString();
    }
}