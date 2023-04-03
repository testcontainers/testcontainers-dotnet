namespace Testcontainers.Dapr;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DaprContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DaprContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DaprContainer(DaprConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    // TODO: I am not sure about the names, maybe we find better ones
    public string GetHttpAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(DaprBuilder.DaprHttpPort)).ToString();
    }

    public string GetGrpcAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(DaprBuilder.DaprGrpcPort)).ToString();
    }
}