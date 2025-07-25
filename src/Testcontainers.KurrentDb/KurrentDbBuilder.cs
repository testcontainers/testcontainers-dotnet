namespace Testcontainers.KurrentDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KurrentDbBuilder : ContainerBuilder<KurrentDbBuilder, KurrentDbContainer, KurrentDbConfiguration>
{
    public const string KurrentDbImage = "kurrentplatform/kurrentdb:25.0";

    public const ushort KurrentDbPort = 2113;

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbBuilder" /> class.
    /// </summary>
    public KurrentDbBuilder()
        : this(new KurrentDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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