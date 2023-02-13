namespace WebDriver;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class WebDriverBuilder : ContainerBuilder<WebDriverBuilder, WebDriverContainer, WebDriverConfiguration>
{
    public const string ChromeStandaloneImage = "selenium/standalone-chrome";
    public const string FirefoxStandaloneImage = "selenium/standalone-firefox";
    public const string EdgeStandaloneImage = "selenium/standalone-edge";
    public const string OperaStandaloneImage = "selenium/standalone-opera";

    public const ushort SeleniumPort = 4444;
    public const ushort VncServerPort = 5900;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    public WebDriverBuilder() : this(new WebDriverConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "WebDriverBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.
        DockerResourceConfiguration = Init().DockerResourceConfiguration;

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "WebDriverBuilder WithWebDriverConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable WebDriverConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebDriverBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private WebDriverBuilder(WebDriverConfiguration resourceConfiguration) : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    protected override WebDriverConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the WebDriver config.
    // /// </summary>
    // /// <param name="config">The WebDriver config.</param>
    // /// <returns>A configured instance of <see cref="WebDriverBuilder" />.</returns>
    // public WebDriverBuilder WithWebDriverConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in WebDriverConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new WebDriverConfiguration(config: config));
    // }

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
            .WithImage(ChromeStandaloneImage)
            .WithPortBinding(SeleniumPort, true);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();
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
}