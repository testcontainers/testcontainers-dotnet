namespace Testcontainers.Floci;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FlociBuilder : ContainerBuilder<FlociBuilder, FlociContainer, FlociConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string FlociImage = "floci/floci:1.5.13";

    public const ushort FlociPort = 4566;

    public const string AccessKey = "test";

    public const string SecretKey = "test";

    public const string Region = "us-east-1";

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public FlociBuilder()
        : this(FlociImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>floci/floci:1.5.13</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/floci/floci/tags" />.
    /// </remarks>
    public FlociBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/floci/floci/tags" />.
    /// </remarks>
    public FlociBuilder(IImage image)
        : this(new FlociConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private FlociBuilder(FlociConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override FlociConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FlociContainer Build()
    {
        Validate();
        return new FlociContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override FlociBuilder Init()
    {
        return base.Init()
            .WithPortBinding(FlociPort, true)
            .WithConnectionStringProvider(new FlociConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/_floci/health").ForPort(FlociPort)));
    }

    /// <inheritdoc />
    protected override FlociBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FlociConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FlociBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FlociConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FlociBuilder Merge(FlociConfiguration oldValue, FlociConfiguration newValue)
    {
        return new FlociBuilder(new FlociConfiguration(oldValue, newValue));
    }
}