namespace Testcontainers.Qdrant;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class QdrantContainer : DockerContainer
{
    private readonly QdrantConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="QdrantContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public QdrantContainer(QdrantConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the connection string for connecting to Qdrant REST APIs.
    /// </summary>
    public string GetHttpConnectionString()
    {
        var scheme = _configuration.TlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        var endpoint = new UriBuilder(
            scheme,
            Hostname,
            GetMappedPublicPort(QdrantBuilder.QdrantHttpPort)
        );
        return endpoint.ToString();
    }

    /// <summary>
    /// Gets the connection string for connecting to Qdrant gRPC APIs.
    /// </summary>
    public string GetGrpcConnectionString()
    {
        var scheme = _configuration.TlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        var endpoint = new UriBuilder(
            scheme,
            Hostname,
            GetMappedPublicPort(QdrantBuilder.QdrantGrpcPort)
        );
        return endpoint.ToString();
    }
}
