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
    /// Gets the Selenium Grid endpoint.
    /// </summary>
    /// <returns>The Selenium Grid endpoint.</returns>
    public Uri GetWebDriverUri()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(WebDriverBuilder.WebDriverPort)).Uri;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    public INetwork GetNetwork()
    {
        return _configuration.Network;
    }

    /// <inheritdoc />
    protected override async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
        await _configuration.Network.CreateAsync(ct)
            .ConfigureAwait(false);

        await base.UnsafeCreateAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeStartAsync(CancellationToken ct = default)
    {
        await base.UnsafeStartAsync(ct)
            .ConfigureAwait(false);

        if (_configuration.FFmpegContainer != null)
        {
            await _configuration.FFmpegContainer.StartAsync(ct)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override async Task UnsafeStopAsync(CancellationToken ct = default)
    {
        if (_configuration.FFmpegContainer != null)
        {
            await _configuration.FFmpegContainer.StopAsync(ct)
                .ConfigureAwait(false);
        }

        await base.UnsafeStopAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
        await base.UnsafeDeleteAsync(ct)
            .ConfigureAwait(false);

        await _configuration.Network.DeleteAsync(ct)
            .ConfigureAwait(false);
    }
}