namespace Testcontainers.JanusGraph;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class JanusGraphBuilder : ContainerBuilder<JanusGraphBuilder, JanusGraphContainer, JanusGraphConfiguration>
{
    public const string JanusGraphImage = "janusgraph/janusgraph:1.0.0";

    public const ushort JanusGraphPort = 8182;

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphBuilder" /> class.
    /// </summary>
    public JanusGraphBuilder()
        : this(new JanusGraphConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
        return new JanusGraphContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override JanusGraphBuilder Init()
    {
        return base.Init()
            .WithImage(JanusGraphImage)
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