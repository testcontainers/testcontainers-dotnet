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
    /// <remarks>
    /// The endpoint has the format <c>host:port</c>, as expected by the
    /// <c>SPANNER_EMULATOR_HOST</c> environment variable.
    /// </remarks>
    /// <returns>The Spanner emulator gRPC endpoint.</returns>
    public string GetEmulatorGrpcEndpoint()
    {
        return Hostname + ":" + GetMappedPublicPort(SpannerBuilder.SpannerGrpcPort);
    }

    /// <summary>
    /// Gets the Spanner emulator HTTP (REST) endpoint.
    /// </summary>
    /// <remarks>
    /// The endpoint has the format <c>host:port</c>.
    /// </remarks>
    /// <returns>The Spanner emulator HTTP endpoint.</returns>
    public string GetEmulatorHttpEndpoint()
    {
        return Hostname + ":" + GetMappedPublicPort(SpannerBuilder.SpannerHttpPort);
    }
}