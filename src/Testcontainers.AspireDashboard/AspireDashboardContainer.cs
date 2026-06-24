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
    ///
    /// </summary>
    /// <returns></returns>
    public string GetDashboardAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardHttpPort)).ToString();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string GetOltpGrpcAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardOltpGrpcPort)).ToString();
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public string GetOltpHttpAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardOltpHttpPort)).ToString();
    }
}