using Microsoft.Extensions.Logging;

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
        Started += (_, _) => Logger.LogInformation("AspireDashboard container is ready!");
        Logger.LogInformation(
            "Dashboard available at {Url}.",
            new UriBuilder(
                Uri.UriSchemeHttp,
                Hostname,
                GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardFrontendPort)
            )
        );
        Logger.LogInformation(
            "OTLP endpoint available at {Url}.",
            new UriBuilder(
                Uri.UriSchemeHttp,
                Hostname,
                GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardOtlpPort)
            )
        );
    }

    /// <summary>
    /// Gets the AspireDashboard URL.
    /// </summary>
    /// <returns>The AspireDashboard URL.</returns>
    public string GetDashboardUrl()
    {
        var endpoint = new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardFrontendPort)
        );

        return endpoint.ToString();
    }

    /// <summary>
    /// Gets the AspireDashboard OTLP endpoint URL.
    /// </summary>
    /// <returns>The AspireDashboard OTLP endpoint URL.</returns>
    public string GetOtlpEndpointUrl()
    {
        var endpoint = new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(AspireDashboardBuilder.AspireDashboardOtlpPort)
        );

        return endpoint.ToString();
    }
}
