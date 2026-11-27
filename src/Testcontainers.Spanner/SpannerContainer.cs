namespace Testcontainers.Spanner;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class SpannerContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpannerContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public SpannerContainer(SpannerConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Spanner emulator gRPC endpoint.
    /// </summary>
    /// <returns>The Spanner emulator gRPC endpoint.</returns>
    public string GetEmulatorGrpcEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(SpannerBuilder.SpannerGrpcPort)).ToString();
    }

    /// <summary>
    /// Gets the Spanner emulator HTTP endpoint.
    /// </summary>
    /// <remarks>
    /// The endpoint serves the REST API of the emulator.
    /// </remarks>
    /// <returns>The Spanner emulator HTTP endpoint.</returns>
    public string GetEmulatorHttpEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(SpannerBuilder.SpannerHttpPort)).ToString();
    }
}