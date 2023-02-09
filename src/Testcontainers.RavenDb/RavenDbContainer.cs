namespace Testcontainers.RavenDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class RavenDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public RavenDbContainer(RavenDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the RavenDb endpoint.
    /// </summary>
    /// <returns>The RavenDb endpoint.</returns>
    public string GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(RavenDbBuilder.RavenDbPort)).ToString();
    }
}