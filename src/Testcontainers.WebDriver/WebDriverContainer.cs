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
    /// Gets the Selenium Grid connection string.
    /// </summary>
    /// <returns>The Selenium Grid connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(WebDriverBuilder.WebDriverPort)).ToString();
    }

    /// <summary>
    /// Gets the Selenium Grid network.
    /// </summary>
    /// <returns>The Selenium Grid network.</returns>
    public INetwork GetNetwork()
    {
        return _configuration.Networks.Single();
    }

    /// <summary>
    /// Exports the recorded video.
    /// </summary>
    /// <param name="target">The target file path to write.</param>
    /// <param name="ct">Cancellation token.</param>
    public async Task ExportVideo(string target, CancellationToken ct = default)
    {
        var bytes = await _configuration.FFmpegContainer.ReadFileAsync(WebDriverBuilder.VideoFilePath, ct)
            .ConfigureAwait(false);

        File.WriteAllBytes(target, bytes);
    }

    /// <inheritdoc />
    protected override async Task UnsafeCreateAsync(CancellationToken ct = default)
    {
        await _configuration.Networks.Single().CreateAsync(ct)
            .ConfigureAwait(false);

        await base.UnsafeCreateAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeStartAsync(CancellationToken ct = default)
    {
        await base.UnsafeStartAsync(ct)
            .ConfigureAwait(false);

        if (_configuration.FFmpegContainer is not null)
        {
            await _configuration.FFmpegContainer.StartAsync(ct)
                .ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    protected override async Task UnsafeStopAsync(CancellationToken ct = default)
    {
        if (_configuration.FFmpegContainer is not null)
        {
            await _configuration.FFmpegContainer.StopAsync(ct)
                .ConfigureAwait(false);
        }

        await base.UnsafeStopAsync(ct)
            .ConfigureAwait(false);
    }
}