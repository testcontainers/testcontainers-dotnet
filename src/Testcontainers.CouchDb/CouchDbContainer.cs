namespace Testcontainers.CouchDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CouchDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public CouchDbContainer(CouchDbConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}