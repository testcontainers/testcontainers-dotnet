namespace Testcontainers.WebDriver;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class WebDriverContainer : DockerContainer
{
    private readonly WebDriverConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public WebDriverContainer(WebDriverConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the uri entry point of the grid.
    /// </summary>
    /// <returns>Uri of selenium grid router component.</returns>
    /// <remarks>
    /// https://www.selenium.dev/documentation/grid/components/#router
    /// </remarks>
    public Uri GetWebDriverUri()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(WebDriverBuilder.WebDriverPort)).Uri;
    }

    /// <summary>
    /// overwrite base StartAsync. With Recording cause: 
    /// Create Network that both base and recording containers use
    /// Start base - WebDriver container and then recording container
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#video-recording
    /// </remarks>
    public override async Task StartAsync(CancellationToken ct = default)
    {
        if (_configuration.Network is not null)
        {
            await _configuration.Network.CreateAsync(ct)
                .ConfigureAwait(false);
        }

        await base.StartAsync(ct)
            .ConfigureAwait(false);

        if (_configuration.Network is not null)
        {
            await _configuration.RecordingContainer.StartAsync(ct)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Overwrite base StopAsync. With Recording cause: 
    /// *Stop* Recording - avoid recording file to be corrupt
    /// Stop base - WebDriver container
    /// Then dispose the recording container
    /// Finally delete the network
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#video-recording
    /// </remarks>
    public override async Task StopAsync(CancellationToken ct = default)
    {
        if (_configuration.RecordingContainer is not null)
        {
            await _configuration.RecordingContainer.StopAsync(ct)
                .ConfigureAwait(false);
        }

        await base.StopAsync(ct)
            .ConfigureAwait(false);

        if (_configuration.RecordingContainer is not null)
        {
            await _configuration.RecordingContainer.DisposeAsync()
                .ConfigureAwait(false);

            await _configuration.Network.DeleteAsync(ct)
                .ConfigureAwait(false);
        }
    }
}