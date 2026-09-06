namespace Testcontainers.MiniStack;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MiniStackBuilder : ContainerBuilder<MiniStackBuilder, MiniStackContainer, MiniStackConfiguration>
{
    [Obsolete(
        "This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string MiniStackImage = "ministackorg/ministack:1.5.8";

    public const ushort MiniStackPort = 4566;

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackBuilder" /> class.
    /// </summary>
    [Obsolete(
        "This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public MiniStackBuilder()
        : this(MiniStackImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>ministackorg/ministack:1.5.8</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ministackorg/ministack/tags" />.
    /// </remarks>
    public MiniStackBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ministackorg/ministack/tags" />.
    /// </remarks>
    public MiniStackBuilder(IImage image)
        : this(new MiniStackConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MiniStackBuilder(MiniStackConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MiniStackConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override MiniStackContainer Build()
    {
        Validate();
        return new MiniStackContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Init()
    {
        return base.Init()
            .WithPortBinding(MiniStackPort, true)
            .WithConnectionStringProvider(new MiniStackConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/_ministack/health").ForPort(MiniStackPort)));
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Merge(MiniStackConfiguration oldValue, MiniStackConfiguration newValue)
    {
        return new MiniStackBuilder(new MiniStackConfiguration(oldValue, newValue));
    }
}
