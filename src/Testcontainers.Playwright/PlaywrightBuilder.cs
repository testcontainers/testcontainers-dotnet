namespace Testcontainers.Playwright;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PlaywrightBuilder : ContainerBuilder<PlaywrightBuilder, PlaywrightContainer, PlaywrightConfiguration>
{
    public const string PlaywrightNetworkAlias = "standalone-container";

    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string PlaywrightImage = "mcr.microsoft.com/playwright:v1.55.1";

    public const ushort PlaywrightPort = 8080;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public PlaywrightBuilder()
        : this(PlaywrightImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://mcr.microsoft.com/en-us/artifact/mar/playwright/tags">https://mcr.microsoft.com/en-us/artifact/mar/playwright/tags</see>.</param>
    public PlaywrightBuilder(string image)
        : this(new PlaywrightConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PlaywrightBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public PlaywrightBuilder(IImage image)
        : this(new PlaywrightConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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
            .WithNetwork(new NetworkBuilder().Build())
            .WithNetworkAliases(PlaywrightNetworkAlias)
            .WithPortBinding(PlaywrightPort, true)
            .WithEntrypoint("/bin/sh", "-c")
            // Extract the Playwright version from the container at startup.
            .WithCommand("npx -y playwright@$(sed --quiet 's/.*\\\"driverVersion\\\": *\"\\([^\"]*\\)\".*/\\1/p' ms-playwright/.docker-info) run-server --port " + PlaywrightPort)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Listening on ws://localhost:8080/"));
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