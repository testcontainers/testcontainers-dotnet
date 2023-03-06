namespace Testcontainers.WebDriver;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class WebDriverBuilder : ContainerBuilder<WebDriverBuilder, WebDriverContainer, WebDriverConfiguration>
{
    public const ushort WebDriverPort = 4444;

    public const ushort VncServerPort = 5900;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    public WebDriverBuilder() : this(new WebDriverConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private WebDriverBuilder(WebDriverConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override WebDriverConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the browser type to run.
    /// </summary>
    /// <param name="webDriverType">Struct represents the browser type to lunch with latest tag.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithBrowser(WebDriverType webDriverType)
    {
        return WithImage(webDriverType.Image);
    }

    /// <summary>
    /// Sets the grid additional commandline parameters for starting.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#se_opts-selenium-configuration-options
    /// </remarks>
    /// <param name="options">The options as a dictionary list divided by comma for starting hub or node.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithConfigurationOptions(IDictionary<string, string> options)
    {
        return WithEnvironment("SE_OPTS", string.Join(",", options.Select(option => string.Join("=", option.Key, option.Value))));
    }

    /// <summary>
    /// Sets the environment variable to java process.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#se_java_opts-java-environment-options
    /// </remarks>
    /// <param name="javaOptions">The java options environment variables as a dictionary list divided by comma.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithJavaEnvironmentOptions(IDictionary<string, string> javaOptions)
    {
        return WithEnvironment("SE_JAVA_OPTS", string.Join(",", javaOptions.Select(option => string.Join("=", option.Key, option.Value))));
    }

    /// <summary>
    /// Sets the screen resolution.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#setting-screen-resolution
    /// </remarks>
    /// <param name="screenWidth">The screen width resolution.</param>
    /// <param name="screenHeight">The screen height resolution.</param>
    /// <param name="screenDepth">The screen depth resolution.</param>
    /// <param name="screenDpi">The screen depth resolution.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SettingScreenResolution(int screenWidth = 1020,
        int screenHeight = 1360, int screenDepth = 24, int screenDpi = 96)
    {
        return WithEnvironment("SE_SCREEN_WIDTH", screenWidth.ToString())
            .WithEnvironment("SE_SCREEN_HEIGHT", screenHeight.ToString())
            .WithEnvironment("SE_SCREEN_DEPTH", screenDepth.ToString())
            .WithEnvironment("SE_SCREEN_DPI", screenDpi.ToString());
    }

    /// <summary>
    /// Sets the grid session timeout until it is killed.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#grid-url-and-session-timeout
    /// </remarks>
    /// <param name="sessionTimeout">The Grid  session timeout config as TimeSpan.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetSessionTimeout(TimeSpan sessionTimeout = default)
    {
        return WithEnvironment("SE_NODE_SESSION_TIMEOUT", sessionTimeout.TotalSeconds.ToString(CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Sets grid time zone by env variable.
    /// </summary>
    /// <param name="timeZone">The desirable time zone.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetTimeZone(TimeZoneInfo timeZone)
    {
        return WithEnvironment("TZ", timeZone.DisplayName);
    }

    /// <summary>
    /// Sets grid configuration by toml file.
    /// </summary>
    /// <remarks>
    /// https://www.selenium.dev/documentation/grid/configuration/toml_options/
    /// </remarks>
    /// <param name="configTomlFilePath">The config toml file path.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetConfigurationFromTomlFile(string configTomlFilePath)
    {
        return WithResourceMapping(configTomlFilePath, "/opt/bin/config.toml");
    }

    /// <summary>
    /// Sets ffmpeg video recording container.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#video-recording
    /// </remarks>
    /// <param name="fileName">video file name - default video name</param>
    /// <param name="fileType">video file type - default mp4 type</param>
    /// <param name="videosFolder">source of video folder on your host - default /tmp/videos path</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithRecording(string fileName = "video", string fileType = "mp4", string videosFolder = "/tmp/videos")
    {
        var webDriverContainerNetworkName = Guid.NewGuid().ToString("D");

        var network = new NetworkBuilder()
            .WithDriver(NetworkDriver.Bridge)
            .Build();

        var recordingContainer = new ContainerBuilder()
            .WithImage(WebDriverType.Video.Image)
            .WithNetwork(network)
            .WithEnvironment("FILE_NAME", $"{fileName}.{fileType}")
            .WithEnvironment("DISPLAY_CONTAINER_NAME", webDriverContainerNetworkName)
            .WithBindMount(videosFolder, "/videos")
            .Build();

        return Merge(DockerResourceConfiguration, new WebDriverConfiguration(network: network, recordingContainer: recordingContainer))
            .WithNetwork(network)
            .WithNetworkAliases(webDriverContainerNetworkName);
    }

    /// <inheritdoc />
    public override WebDriverContainer Build()
    {
        Validate();
        return new WebDriverContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Init()
    {
        return base.Init()
            .WithBrowser(WebDriverType.Chrome)
            .WithPortBinding(WebDriverPort, true)
            .WithPortBinding(VncServerPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/wd/hub/status")
                    .ForPort(WebDriverPort)
                    .ForResponseMessageMatching(IsGridReadyAsync)
            ));
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WebDriverConfiguration(resourceConfiguration));
    }

    /// <inheritdoc /> 
    protected override WebDriverBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new WebDriverConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Merge(WebDriverConfiguration oldValue, WebDriverConfiguration newValue)
    {
        return new WebDriverBuilder(new WebDriverConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Determines whether the selenium grid is up and ready to receive requests.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#waiting-for-the-grid-to-be-ready
    /// </remarks>
    /// <param name="response">The HTTP response that contains the hub and nodes information.</param>
    /// <returns>A value indicating whether the selenium grid is ready.</returns>
    private async Task<bool> IsGridReadyAsync(HttpResponseMessage response)
    {
        var jsonString = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        try
        {
            return JsonDocument.Parse(jsonString)
                .RootElement
                .GetProperty("value")
                .GetProperty("ready")
                .GetBoolean();
        }
        catch
        {
            return false;
        }
    }
}