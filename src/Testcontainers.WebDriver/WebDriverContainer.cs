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
}