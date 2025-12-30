namespace Testcontainers.JanusGraph;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class JanusGraphBuilder : ContainerBuilder<JanusGraphBuilder, JanusGraphContainer, JanusGraphConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string JanusGraphImage = "janusgraph/janusgraph:1.0.0";

    public const ushort JanusGraphPort = 8182;

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public JanusGraphBuilder()
        : this(JanusGraphImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>janusgraph/janusgraph:1.0.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/janusgraph/janusgraph/tags" />.
    /// </remarks>
    public JanusGraphBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/janusgraph/janusgraph/tags" />.
    /// </remarks>
    public JanusGraphBuilder(IImage image)
        : this(new JanusGraphConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private JanusGraphBuilder(JanusGraphConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override JanusGraphConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override JanusGraphContainer Build()
    {
        Validate();
        return new JanusGraphContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override JanusGraphBuilder Init()
    {
        return base.Init()
            .WithPortBinding(JanusGraphPort, true)
            .WithEnvironment("janusgraph.storage.backend", "inmemory")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Channel started at port"));
    }

    /// <inheritdoc />
    protected override JanusGraphBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new JanusGraphConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override JanusGraphBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new JanusGraphConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override JanusGraphBuilder Merge(JanusGraphConfiguration oldValue, JanusGraphConfiguration newValue)
    {
        return new JanusGraphBuilder(new JanusGraphConfiguration(oldValue, newValue));
    }
}