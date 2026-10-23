namespace Testcontainers.Chroma;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ChromaBuilder : ContainerBuilder<ChromaBuilder, ChromaContainer, ChromaConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string ChromaImage = "chromadb/chroma:1.5.9";

    public const ushort ChromaHttpPort = 8000;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public ChromaBuilder()
        : this(ChromaImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>chromadb/chroma:1.5.9</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/chromadb/chroma/tags" />.
    /// </remarks>
    public ChromaBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/chromadb/chroma/tags" />.
    /// </remarks>
    public ChromaBuilder(IImage image)
        : this(new ChromaConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ChromaBuilder(ChromaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ChromaConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override ChromaContainer Build()
    {
        Validate();
        return new ChromaContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ChromaBuilder Init()
    {
        return base.Init()
            .WithPortBinding(ChromaHttpPort, true)
            .WithConnectionStringProvider(new ChromaConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()));
    }

    /// <inheritdoc />
    protected override ChromaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ChromaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ChromaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ChromaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ChromaBuilder Merge(ChromaConfiguration oldValue, ChromaConfiguration newValue)
    {
        return new ChromaBuilder(new ChromaConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    /// <remarks>
    /// Chroma is ready when its heartbeat answers. Chroma 0.5.16 and later answer the heartbeat of the v2 API,
    /// the earlier releases have only the v1 API, and Chroma 1.x answers the v1 API with <c>410 Gone</c>.
    /// Asking both works on every release, without reading the version from the image tag.
    /// </remarks>
    private sealed class WaitUntil : IWaitUntil
    {
        private static readonly IWaitUntil V2Heartbeat = new HttpWaitStrategy()
            .ForPort(ChromaHttpPort)
            .ForPath("/api/v2/heartbeat");

        private static readonly IWaitUntil V1Heartbeat = new HttpWaitStrategy()
            .ForPort(ChromaHttpPort)
            .ForPath("/api/v1/heartbeat");

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            return await V2Heartbeat.UntilAsync(container)
                .ConfigureAwait(false) || await V1Heartbeat.UntilAsync(container)
                .ConfigureAwait(false);
        }
    }
}
