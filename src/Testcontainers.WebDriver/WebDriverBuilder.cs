namespace Testcontainers.WebDriver;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// Find further information about the Selenium Grid image, including its configurations, here: https://github.com/SeleniumHQ/docker-selenium.
/// </remarks>
[PublicAPI]
public sealed class WebDriverBuilder : ContainerBuilder<WebDriverBuilder, WebDriverContainer, WebDriverConfiguration>
{
    public const string WebDriverNetworkAlias = "standalone-container";

    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string FFmpegNetworkAlias = "ffmpeg-container";

    public const string FFmpegImage = "selenium/video:ffmpeg-4.3.1-20230306";

    public const ushort WebDriverPort = 4444;

    public const ushort VncServerPort = 5900;

    public static readonly string VideoFilePath = string.Join("/", string.Empty, "videos", "video.mp4");

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public WebDriverBuilder()
        : this(WebDriverBrowser.Chrome.Image)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>selenium/standalone-chrome:110.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags for standalone Selenium browsers:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             Chrome: <see href="https://hub.docker.com/r/selenium/standalone-chrome/tags" />.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Firefox: <see href="https://hub.docker.com/r/selenium/standalone-firefox/tags" />.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Edge: <see href="https://hub.docker.com/r/selenium/standalone-edge/tags" />.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    public WebDriverBuilder(string image)
        : this(new WebDriverConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags for standalone Selenium browsers:
    /// <list type="bullet">
    ///     <item>
    ///         <description>
    ///             Chrome: <see href="https://hub.docker.com/r/selenium/standalone-chrome/tags" />.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Firefox: <see href="https://hub.docker.com/r/selenium/standalone-firefox/tags" />.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <description>
    ///             Edge: <see href="https://hub.docker.com/r/selenium/standalone-edge/tags" />.
    ///         </description>
    ///     </item>
    /// </list>
    /// </remarks>
    public WebDriverBuilder(IImage image)
        : this(new WebDriverConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private WebDriverBuilder(WebDriverConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override WebDriverConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Web Driver browser configuration.
    /// </summary>
    /// <remarks>
    /// https://www.selenium.dev/documentation/webdriver/browsers/
    /// </remarks>
    /// <param name="webDriverBrowser">The Web Driver browser configuration.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithBrowser(WebDriverBrowser webDriverBrowser)
    {
        return WithImage(webDriverBrowser.Image);
    }

    /// <summary>
    /// Overrides the Selenium Grid configurations with the TOML file.
    /// </summary>
    /// <remarks>
    /// https://www.selenium.dev/documentation/grid/configuration/toml_options/.
    /// </remarks>
    /// <param name="configTomlFilePath">The TOML file.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithConfigurationFromTomlFile(string configTomlFilePath)
    {
        return WithResourceMapping(new FileInfo(configTomlFilePath), new FileInfo("/opt/bin/config.toml"));
    }

    /// <summary>
    /// Enables the video recording.
    /// </summary>
    /// <param name="ffmpegImage">The FFmpeg image to use for recording.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithRecording(string ffmpegImage = FFmpegImage)
    {
        var ffmpegContainer = new ContainerBuilder(ffmpegImage)
            .WithNetwork(DockerResourceConfiguration.Networks.Single())
            .WithNetworkAliases(FFmpegNetworkAlias)
            .WithEnvironment("FILE_NAME", Path.GetFileName(VideoFilePath))
            .WithEnvironment("DISPLAY_CONTAINER_NAME", WebDriverNetworkAlias)
            .Build();

        return Merge(DockerResourceConfiguration, new WebDriverConfiguration(ffmpegContainer: ffmpegContainer));
    }

    /// <inheritdoc />
    public override WebDriverContainer Build()
    {
        Validate();
        return new WebDriverContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override WebDriverBuilder Init()
    {
        return base.Init()
            .WithBrowser(WebDriverBrowser.Chrome)
            .WithNetwork(new NetworkBuilder().Build())
            .WithNetworkAliases(WebDriverNetworkAlias)
            .WithPortBinding(WebDriverPort, true)
            .WithPortBinding(VncServerPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/wd/hub/status").ForPort(WebDriverPort).ForResponseMessageMatching(IsGridReadyAsync)));
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
    /// Determines whether the Selenium Grid is up and ready to receive requests.
    /// </summary>
    /// <remarks>
    /// https://github.com/SeleniumHQ/docker-selenium#waiting-for-the-grid-to-be-ready.
    /// </remarks>
    /// <param name="response">The HTTP response that contains the Selenium Grid information.</param>
    /// <returns>A value indicating whether the Selenium Grid is ready.</returns>
    private static async Task<bool> IsGridReadyAsync(HttpResponseMessage response)
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