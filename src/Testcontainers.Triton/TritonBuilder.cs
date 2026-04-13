namespace Testcontainers.Triton;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class TritonBuilder : ContainerBuilder<TritonBuilder, TritonContainer, TritonConfiguration>
{
    public const ushort TritonHttpPort = 8000;

    public const ushort TritonGrpcPort = 8001;

    public const ushort TritonMetricsPort = 8002;

    /// <summary>
    /// Initializes a new instance of the <see cref="TritonBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>nvcr.io/nvidia/tritonserver:25.12-py3</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://catalog.ngc.nvidia.com/orgs/nvidia/containers/tritonserver/tags" />.
    /// </remarks>
    public TritonBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TritonBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://catalog.ngc.nvidia.com/orgs/nvidia/containers/tritonserver/tags" />.
    /// </remarks>
    public TritonBuilder(IImage image)
        : this(new TritonConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TritonBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private TritonBuilder(TritonConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override TritonConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override TritonContainer Build()
    {
        Validate();
        return new TritonContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override TritonBuilder Init()
    {
        return base.Init()
            .WithPortBinding(TritonHttpPort, true)
            .WithPortBinding(TritonGrpcPort, true)
            .WithPortBinding(TritonMetricsPort, true)
            .WithConnectionStringProvider(new TritonConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("Started GRPCInferenceService"));
    }

    /// <inheritdoc />
    protected override TritonBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TritonConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TritonBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new TritonConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override TritonBuilder Merge(TritonConfiguration oldValue, TritonConfiguration newValue)
    {
        return new TritonBuilder(new TritonConfiguration(oldValue, newValue));
    }
}
