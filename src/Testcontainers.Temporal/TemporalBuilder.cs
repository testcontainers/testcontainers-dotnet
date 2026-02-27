namespace Testcontainers.Temporal;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class TemporalBuilder : ContainerBuilder<TemporalBuilder, TemporalContainer, TemporalConfiguration>
{
    public const ushort TemporalGrpcPort = 7233;

    public const ushort TemporalHttpPort = 8233;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>temporalio/temporal:1.5.1</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/temporalio/temporal/tags" />.
    /// </remarks>
    public TemporalBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/temporalio/temporal/tags" />.
    /// </remarks>
    public TemporalBuilder(IImage image)
        : this(new TemporalConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private TemporalBuilder(TemporalConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override TemporalConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override TemporalContainer Build()
    {
        Validate();
        return new TemporalContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override TemporalBuilder Init()
    {
        return base.Init()
            .WithPortBinding(TemporalGrpcPort, true)
            .WithPortBinding(TemporalHttpPort, true)
            .WithCommand("server", "start-dev", "--ip", "0.0.0.0")
            .WithConnectionStringProvider(new TemporalConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilExternalTcpPortIsAvailable(TemporalGrpcPort)
                .UntilExternalTcpPortIsAvailable(TemporalHttpPort)
                .UntilHttpRequestIsSucceeded(request =>
                    request.ForPath("/api/v1/system-info").ForPort(TemporalHttpPort)));
    }

    /// <inheritdoc />
    protected override TemporalBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TemporalBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TemporalConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TemporalBuilder Merge(TemporalConfiguration oldValue, TemporalConfiguration newValue)
    {
        return new TemporalBuilder(new TemporalConfiguration(oldValue, newValue));
    }
}