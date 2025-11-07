namespace Testcontainers.Grafana;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class GrafanaContainer : DockerContainer
{
    private readonly GrafanaConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="GrafanaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public GrafanaContainer(GrafanaConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Grafana base address.
    /// </summary>
    /// <returns>The Grafana base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(GrafanaBuilder.GrafanaPort)).ToString();
    }
}