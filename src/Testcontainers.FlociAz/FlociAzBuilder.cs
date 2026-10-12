namespace Testcontainers.FlociAz;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FlociAzBuilder : ContainerBuilder<FlociAzBuilder, FlociAzContainer, FlociAzConfiguration>
{
    private const string DockerSocket = "/var/run/docker.sock";

    public const ushort FlociAzPort = 4577;

    public const string AccountName = "devstoreaccount1";

    public const string AccountKey = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";

    /// <summary>
    /// Initializes a new instance of the <see cref="FlociAzBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>floci/floci-az:0.12.0</c>).
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

    /// <summary>
    /// Grants FlociAz access to the Docker daemon for services that use sidecar containers.
    /// </summary>
    /// <remarks>
    /// The Docker socket provides root-equivalent access to the Docker host. Only enable it for
    /// trusted images. FlociAz child containers and volumes receive a unique namespace that is
    /// registered with the Testcontainers Resource Reaper.
    /// </remarks>
    /// <param name="dockerSocket">The host Docker socket path, or <c>null</c> to detect it.</param>
    /// <returns>A configured instance of <see cref="FlociAzBuilder" />.</returns>
    public FlociAzBuilder WithDockerSocket(string dockerSocket = null)
    {
        var endpoint = DockerResourceConfiguration.DockerEndpointAuthConfig.Endpoint;
        var detectedSocket = endpoint.Scheme.Equals("unix", StringComparison.OrdinalIgnoreCase) ? endpoint.AbsolutePath : DockerSocket;
        var source = dockerSocket ?? TestcontainersSettings.DockerSocketOverride ?? detectedSocket;
        var resourceNamespace = "tc-" + Guid.NewGuid().ToString("N");

        return WithBindMount(source, DockerSocket, AccessMode.ReadWrite)
            .WithEnvironment("FLOCI_AZ_DOCKER_RESOURCE_NAMESPACE", resourceNamespace);
    }

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
            .WithEnvironment("FLOCI_AZ_SERVICES_EVENT_HUB_ENABLED", "false")
            .WithEnvironment("FLOCI_AZ_SERVICES_FUNCTIONS_MOCKED", "true")
            .WithEnvironment("FLOCI_AZ_SERVICES_POSTGRES_MOCKED", "true")
            .WithEnvironment("FLOCI_AZ_SERVICES_AKS_MOCKED", "true")
            .WithEnvironment("FLOCI_AZ_SERVICES_ACR_MOCKED", "true")
            .WithEnvironment("FLOCI_AZ_SERVICES_REDIS_MOCKED", "true")
            .WithEnvironment("FLOCI_AZ_SERVICES_SERVICE_BUS_MOCKED", "true")
            .WithEnvironment("FLOCI_AZ_SERVICES_COSMOS_MOCKED", "true")
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
