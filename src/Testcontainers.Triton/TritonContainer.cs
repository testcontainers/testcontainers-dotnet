namespace Testcontainers.Triton;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class TritonContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TritonContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public TritonContainer(TritonConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Triton HTTP endpoint URL.
    /// </summary>
    /// <returns>The Triton HTTP endpoint.</returns>
    public string GetHttpEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(TritonBuilder.TritonHttpPort)).ToString();
    }

    /// <summary>
    /// Gets the Triton gRPC endpoint URL.
    /// </summary>
    /// <returns>The Triton gRPC endpoint.</returns>
    public string GetGrpcEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(TritonBuilder.TritonGrpcPort)).ToString();
    }
}
