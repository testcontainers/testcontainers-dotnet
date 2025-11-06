namespace Testcontainers.Playwright;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PlaywrightBuilder : ContainerBuilder<PlaywrightBuilder, PlaywrightContainer, PlaywrightConfiguration>
{
    public const string PlaywrightNetworkAlias = "standalone-container";

    public const ushort PlaywrightPort = 53333;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBuilder" /> class.
    /// </summary>
    public PlaywrightBuilder()
        : this(new PlaywrightConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PlaywrightBuilder(PlaywrightConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PlaywrightConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Playwright browser configuration.
    /// </summary>
    /// <remarks>
    /// https://github.com/JacobLinCool/playwright-docker.
    /// </remarks>
    /// <param name="playwrightBrowser">The Playwright browser configuration.</param>
    /// <returns>A configured instance of <see cref="PlaywrightBuilder" />.</returns>
    public PlaywrightBuilder WithBrowser(PlaywrightBrowser playwrightBrowser)
    {
        return WithImage(playwrightBrowser.Image);
    }

    /// <inheritdoc />
    public override PlaywrightContainer Build()
    {
        Validate();
        return new PlaywrightContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override PlaywrightBuilder Init()
    {
        return base.Init()
            .WithBrowser(PlaywrightBrowser.Chrome)
            .WithNetwork(new NetworkBuilder().Build())
            .WithNetworkAliases(PlaywrightNetworkAlias)
            .WithPortBinding(PlaywrightPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("ws://localhost:" + PlaywrightPort + "/playwright"));
    }

    /// <inheritdoc />
    protected override PlaywrightBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PlaywrightConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PlaywrightBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PlaywrightConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PlaywrightBuilder Merge(PlaywrightConfiguration oldValue, PlaywrightConfiguration newValue)
    {
        return new PlaywrightBuilder(new PlaywrightConfiguration(oldValue, newValue));
    }
}