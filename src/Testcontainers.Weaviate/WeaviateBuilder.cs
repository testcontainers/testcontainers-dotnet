namespace Testcontainers.Weaviate;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class WeaviateBuilder : ContainerBuilder<WeaviateBuilder, WeaviateContainer, WeaviateConfiguration>
{
    public const string WeaviateImage = "semitechnologies/weaviate:1.26.14";

    public const ushort WeaviateHttpPort = 8080;

    public const ushort WeaviateGrpcPort = 50051;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateBuilder" /> class.
    /// </summary>
    public WeaviateBuilder() : this(new WeaviateConfiguration())
        => DockerResourceConfiguration = Init().DockerResourceConfiguration;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private WeaviateBuilder(WeaviateConfiguration resourceConfiguration) : base(resourceConfiguration)
        => DockerResourceConfiguration = resourceConfiguration;

    /// <inheritdoc />
    protected override WeaviateConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override WeaviateContainer Build()
    {
        Validate();
        return new WeaviateContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override WeaviateBuilder Init()
        => base.Init()
            .WithImage(WeaviateImage)
            .WithPortBinding(WeaviateHttpPort, true)
            .WithPortBinding(WeaviateGrpcPort, true)
            .WithEnvironment("AUTHENTICATION_ANONYMOUS_ACCESS_ENABLED", "true")
            .WithEnvironment("PERSISTENCE_DATA_PATH", "/var/lib/weaviate")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilPortIsAvailable(WeaviateHttpPort)
                .UntilPortIsAvailable(WeaviateGrpcPort)
                .UntilHttpRequestIsSucceeded(request =>
                    request.ForPath("/v1/.well-known/ready").ForPort(WeaviateHttpPort)));

    /// <inheritdoc />
    protected override WeaviateBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new WeaviateConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override WeaviateBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new WeaviateConfiguration(resourceConfiguration));

    /// <inheritdoc />
    protected override WeaviateBuilder Merge(WeaviateConfiguration oldValue, WeaviateConfiguration newValue)
        => new(new WeaviateConfiguration(oldValue, newValue));
}