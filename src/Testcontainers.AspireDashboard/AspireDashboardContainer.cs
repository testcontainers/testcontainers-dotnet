namespace Testcontainers.AspireDashboard;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class AspireDashboardContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AspireDashboardContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public AspireDashboardContainer(AspireDashboardConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Aspire dashboard address.
    /// </summary>
    /// <returns>The Aspire dashboard address.</returns>
    public string GetDashboardAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardHttpPort)).ToString();
    }

    /// <summary>
    /// Gets the OTLP gRPC endpoint address.
    /// </summary>
    /// <returns>The OTLP gRPC endpoint address.</returns>
    public string GetOtlpGrpcAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardOtlpGrpcPort)).ToString();
    }

    /// <summary>
    /// Gets the OTLP HTTP endpoint address.
    /// </summary>
    /// <returns>The OTLP HTTP endpoint address.</returns>
    public string GetOtlpHttpAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardOtlpHttpPort)).ToString();
    }
}