namespace Testcontainers.DragonflyDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DragonflyDbBuilder : ContainerBuilder<DragonflyDbBuilder, DragonflyDbContainer, DragonflyDbConfiguration>
{
    public const string DragonflyDbImage = "docker.dragonflydb.io/dragonflydb/dragonfly";

    public const ushort DragonflyDbPort = 6379;

    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbBuilder" /> class.
    /// </summary>
    public DragonflyDbBuilder()
        : this(new DragonflyDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private DragonflyDbBuilder(DragonflyDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override DragonflyDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override DragonflyDbContainer Build()
    {
        Validate();
        return new DragonflyDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override DragonflyDbBuilder Init()
    {
        return base.Init()
            .WithImage(DragonflyDbImage)
            .WithPortBinding(DragonflyDbPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("/usr/local/bin/healthcheck.sh"));
    }

    /// <inheritdoc />
    protected override DragonflyDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DragonflyDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DragonflyDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DragonflyDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DragonflyDbBuilder Merge(DragonflyDbConfiguration oldValue, DragonflyDbConfiguration newValue)
    {
        return new DragonflyDbBuilder(new DragonflyDbConfiguration(oldValue, newValue));
    }
}