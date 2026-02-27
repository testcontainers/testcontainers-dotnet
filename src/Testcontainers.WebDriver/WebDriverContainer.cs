namespace Testcontainers.WebDriver;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class WebDriverContainer : DockerContainer
{
    private readonly WebDriverConfiguration _configuration;

    private readonly IContainer _ffmpegContainer;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public WebDriverContainer(WebDriverConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
        _ffmpegContainer = configuration.FFmpegContainer ?? FFmpegContainer.Instance;
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
    /// <remarks>
    /// To export the video, the container must be stopped.
    /// </remarks>
    /// <param name="target">The target file path to write.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">The video recording is either not enabled or the container has not been stopped.</exception>
    public async Task ExportVideoAsync(string target, CancellationToken ct = default)
    {
        Guard.Argument(_ffmpegContainer.State, nameof(_ffmpegContainer.State))
            .ThrowIf(argument => TestcontainersStates.Undefined.Equals(argument.Value), _ => new InvalidOperationException("Could not export video. Please enable the video recording first."));

        Guard.Argument(_ffmpegContainer.State, nameof(_ffmpegContainer.State))
            .ThrowIf(argument => !TestcontainersStates.Exited.Equals(argument.Value), _ => new InvalidOperationException("Could not export video. Please stop the WebDriver container first."));

        var bytes = await _ffmpegContainer.ReadFileAsync(WebDriverBuilder.VideoFilePath, ct)
            .ConfigureAwait(false);

#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
        await File.WriteAllBytesAsync(target, bytes, ct)
            .ConfigureAwait(false);
#else
        File.WriteAllBytes(target, bytes);
#endif
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
    protected override Task UnsafeDeleteAsync(CancellationToken ct = default)
    {
        return Task.WhenAll(_ffmpegContainer.DisposeAsync().AsTask(), base.UnsafeDeleteAsync(ct));
    }

    /// <inheritdoc />
    protected override async Task UnsafeStartAsync(CancellationToken ct = default)
    {
        await base.UnsafeStartAsync(ct)
            .ConfigureAwait(false);

        await _ffmpegContainer.StartAsync(ct)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async Task UnsafeStopAsync(CancellationToken ct = default)
    {
        await _ffmpegContainer.StopAsync(ct)
            .ConfigureAwait(false);

        await base.UnsafeStopAsync(ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// A default FFmpeg container to avoid null-coalescing.
    /// </summary>
    private sealed class FFmpegContainer : DockerContainer
    {
        static FFmpegContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FFmpegContainer" /> class.
        /// </summary>
        private FFmpegContainer()
            : base(new ContainerConfiguration(new ResourceConfiguration<CreateContainerParameters>(new DockerEndpointAuthenticationConfiguration(new Uri("tcp://ffmpeg")), null, null, false, NullLogger.Instance)))
        {
        }

        /// <summary>
        /// Gets the <see cref="IContainer" /> instance.
        /// </summary>
        public static IContainer Instance { get; }
            = new FFmpegContainer();

        /// <inheritdoc />
        public override Task StartAsync(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override Task StopAsync(CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }
    }
}