using System.Net.Http;
using System.Threading.Tasks;

namespace Testcontainers.WebDriver;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class WebDriverBuilder : ContainerBuilder<WebDriverBuilder, WebDriverContainer, WebDriverConfiguration>
{
    public const ushort HubRouteServicePort = 4444;

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
    /// <param name="browserType">Struct represents the browser type to lunch with latest tag.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithBrowser(BrowserType browserType)
    {
        return WithImage(browserType.BrowserName);
    }

    /// <summary>
    /// Sets the WebDriver config.
    /// </summary>
    /// <param name="options">The options as string list for starting a hub or a node.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithConfigurationOptions(string options)
    {
        return WithEnvironment("SE_OPTS", options);
    }

    /// <summary>
    /// Sets the WebDriver config.
    /// </summary>
    /// <param name="javaOptions">The java options environment variables as string list.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder WithJavaEnvironmentOptions(string javaOptions)
    {
        return WithEnvironment("SE_JAVA_OPTS", javaOptions);
    }

    /// <summary>
    /// Sets the WebDriver config.
    /// </summary>
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
    /// Sets the WebDriver config.
    /// </summary>
    /// <param name="sessionTimeout">The Grid  session timeout config.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetSessionTimeout(int sessionTimeout = 300)
    {
        return WithEnvironment("SE_NODE_SESSION_TIMEOUT", sessionTimeout.ToString());
    }

    /// <summary>
    /// Sets the WebDriver config.
    /// </summary>
    /// <param name="timeZone">The desirable time zone.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetTimeZone(string timeZone)
    {
        return WithEnvironment("TZ", timeZone);
    }

    /// <summary>
    /// Sets the WebDriver config.
    /// </summary>
    /// <param name="configTomlFilePath">The config toml file path.</param>
    /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    public WebDriverBuilder SetConfigurationFromTomlFile(string configTomlFilePath)
    {
        return WithResourceMapping(configTomlFilePath, "/opt/bin/config.toml");
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
            .WithImage(BrowserType.Chrome.BrowserName)
            .WithPortBinding(HubRouteServicePort, true)
            .WithPortBinding(VncServerPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request
                => request
                    .ForPath("/wd/hub/status")
                    .ForPort(HubRouteServicePort)
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
            var isReady = JsonDocument.Parse(jsonString)
                .RootElement
                .GetProperty("value")
                .GetProperty("ready")
                .GetBoolean();

            return isReady;
        }
        catch
        {
            return false;
        }
    }
}