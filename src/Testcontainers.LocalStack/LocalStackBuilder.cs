namespace Testcontainers.LocalStack;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class LocalStackBuilder : ContainerBuilder<LocalStackBuilder, LocalStackContainer, LocalStackConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string LocalStackImage = "localstack/localstack:2.0";

    public const ushort LocalStackPort = 4566;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public LocalStackBuilder()
        : this(LocalStackImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>localstack/localstack:2.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/localstack/localstack/tags" />.
    /// </remarks>
    public LocalStackBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/localstack/localstack/tags" />.
    /// </remarks>
    public LocalStackBuilder(IImage image)
        : this(new LocalStackConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private LocalStackBuilder(LocalStackConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override LocalStackConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override LocalStackContainer Build()
    {
        Validate();
        return new LocalStackContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Init()
    {
        return base.Init()
            .WithPortBinding(LocalStackPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/_localstack/health").ForPort(LocalStackPort)));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Merge(LocalStackConfiguration oldValue, LocalStackConfiguration newValue)
    {
        return new LocalStackBuilder(new LocalStackConfiguration(oldValue, newValue));
    }
}