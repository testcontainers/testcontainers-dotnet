namespace Testcontainers.Chroma;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ChromaBuilder : ContainerBuilder<ChromaBuilder, ChromaContainer, ChromaConfiguration>
{
    public const ushort ChromaPort = 8000;

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
            .WithPortBinding(ChromaPort, true)
            .WithConnectionStringProvider(() => new ChromaConnectionStringProvider())
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
    /// Uses the heartbeat endpoint to detect readiness of the Chroma container.
    /// Chroma 0.5.16 introduced the v2 API, and Chroma 1.0.0 removed the v1 API.
    /// The wait strategy checks the v2 heartbeat first and falls back to the v1
    /// heartbeat for versions prior to 0.5.16.
    /// </remarks>
    private sealed class WaitUntil : IWaitUntil
    {
        private static readonly IWaitUntil WaitUntilV2HeartbeatIsSucceeded = new HttpWaitStrategy()
            .ForPath("/api/v2/heartbeat")
            .ForPort(ChromaPort);

        private static readonly IWaitUntil WaitUntilV1HeartbeatIsSucceeded = new HttpWaitStrategy()
            .ForPath("/api/v1/heartbeat")
            .ForPort(ChromaPort);

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var v2HeartbeatSucceeded = await WaitUntilV2HeartbeatIsSucceeded.UntilAsync(container)
                .ConfigureAwait(false);

            if (v2HeartbeatSucceeded)
            {
                return true;
            }

            return await WaitUntilV1HeartbeatIsSucceeded.UntilAsync(container)
                .ConfigureAwait(false);
        }
    }
}