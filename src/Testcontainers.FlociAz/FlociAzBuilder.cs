namespace Testcontainers.FlociAz;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FlociAzBuilder : ContainerBuilder<FlociAzBuilder, FlociAzContainer, FlociAzConfiguration>
{
    public const ushort FlociAzPort = 4577;

    public const ushort EventHubsPort = 5672;

    public const ushort ServiceBusPort = 5673;

    public const string AccountName = "devstoreaccount1";

    public const string AccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>floci/floci-az:0.9.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/floci/floci-az/tags" />.
    /// </remarks>
    public FlociAzBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/floci/floci-az/tags" />.
    /// </remarks>
    public FlociAzBuilder(IImage image)
        : this(new FlociAzConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private FlociAzBuilder(FlociAzConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override FlociAzConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override FlociAzContainer Build()
    {
        Validate();
        return new FlociAzContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override FlociAzBuilder Init()
    {
        return base.Init()
            .WithPortBinding(FlociAzPort, true)
            .WithPortBinding(EventHubsPort, true)
            .WithPortBinding(ServiceBusPort, true)
            .WithConnectionStringProvider(new FlociAzConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/_floci/health").ForPort(FlociAzPort)));
    }

    /// <inheritdoc />
    protected override FlociAzBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FlociAzConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FlociAzBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FlociAzConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FlociAzBuilder Merge(FlociAzConfiguration oldValue, FlociAzConfiguration newValue)
    {
        return new FlociAzBuilder(new FlociAzConfiguration(oldValue, newValue));
    }
}
