namespace Testcontainers.KurrentDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KurrentDbBuilder : ContainerBuilder<KurrentDbBuilder, KurrentDbContainer, KurrentDbConfiguration>
{
    public const string KurrentDbImage = "kurrentplatform/kurrentdb:25.1";

    public const ushort KurrentDbPort = 2113;

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    [Obsolete("Use constructor with image as a parameter instead.")]
    public KurrentDbBuilder()
        : this(new KurrentDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(KurrentDbImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/r/kurrentplatform/kurrentdb/tags">https://hub.docker.com/r/kurrentplatform/kurrentdb/tags</see>.</param>
    public KurrentDbBuilder(string image)
        : this(new KurrentDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public KurrentDbBuilder(IImage image)
        : this(new KurrentDbConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private KurrentDbBuilder(KurrentDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override KurrentDbConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override KurrentDbContainer Build()
    {
        return new KurrentDbContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Init()
    {
        return base.Init()
            .WithImage(KurrentDbImage)
            .WithPortBinding(KurrentDbPort, true)
            .WithEnvironment("KURRENTDB_CLUSTER_SIZE", "1")
            .WithEnvironment("KURRENTDB_RUN_PROJECTIONS", "All")
            .WithEnvironment("KURRENTDB_START_STANDARD_PROJECTIONS", "true")
            .WithEnvironment("KURRENTDB_INSECURE", "true")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KurrentDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KurrentDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KurrentDbBuilder Merge(KurrentDbConfiguration oldValue, KurrentDbConfiguration newValue)
    {
        return new KurrentDbBuilder(new KurrentDbConfiguration(oldValue, newValue));
    }
}