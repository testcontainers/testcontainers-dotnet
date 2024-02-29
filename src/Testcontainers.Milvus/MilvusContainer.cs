namespace Testcontainers.Milvus;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MilvusContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MilvusContainer(MilvusConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the Milvus endpoint.
    /// </summary>
    /// <returns>The Milvus endpoint.</returns>
    public Uri GetEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(MilvusBuilder.MilvusGrpcPort)).Uri;
    }
}