namespace Testcontainers.Couchbase;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CouchbaseContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public CouchbaseContainer(CouchbaseConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}