namespace Testcontainers.WebDriver;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class WebDriverContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public WebDriverContainer(WebDriverConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the uri entry point of the grid.
    /// </summary>
    /// <returns>The PostgreSql connection string.</returns>
    public Uri GetWebDriverUri()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(WebDriverPort)).Uri;
    }
}