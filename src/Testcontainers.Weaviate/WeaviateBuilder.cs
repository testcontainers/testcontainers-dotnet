namespace Testcontainers.Weaviate;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class WeaviateBuilder : ContainerBuilder<WeaviateBuilder, WeaviateContainer, WeaviateConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string WeaviateImage = "semitechnologies/weaviate:1.26.14";

    public const ushort WeaviateHttpPort = 8080;

    public const ushort WeaviateGrpcPort = 50051;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public WeaviateBuilder() : this(WeaviateImage) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>semitechnologies/weaviate:1.26.14</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/semitechnologies/weaviate/tags" />.
    /// </remarks>
    public WeaviateBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/semitechnologies/weaviate/tags" />.
    /// </remarks>
    public WeaviateBuilder(IImage image)
        : this(new WeaviateConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

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
            .WithPortBinding(WeaviateHttpPort, true)
            .WithPortBinding(WeaviateGrpcPort, true)
            .WithEnvironment("AUTHENTICATION_ANONYMOUS_ACCESS_ENABLED", "true")
            .WithEnvironment("PERSISTENCE_DATA_PATH", "/var/lib/weaviate")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilInternalTcpPortIsAvailable(WeaviateHttpPort)
                .UntilInternalTcpPortIsAvailable(WeaviateGrpcPort)
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