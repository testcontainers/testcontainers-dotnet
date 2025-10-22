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
    /// Gets the Grafana HTTP endpoint.
    /// </summary>
    /// <returns>The Grafana HTTP endpoint.</returns>
    public string GetHttpEndpoint()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(GrafanaBuilder.GrafanaPort)).ToString();
    }

    /// <summary>
    /// Gets the Grafana connection string.
    /// </summary>
    /// <returns>The Grafana connection string.</returns>
    public string GetConnectionString()
    {
        var endpoint = GetHttpEndpoint();
        var username = _configuration.Username ?? GrafanaBuilder.DefaultUsername;
        var password = _configuration.Password ?? GrafanaBuilder.DefaultPassword;
        return new UriBuilder(endpoint)
        {
            UserName = username,
            Password = password
        }.ToString();
    }
}