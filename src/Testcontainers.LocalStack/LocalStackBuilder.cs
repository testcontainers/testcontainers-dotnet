namespace Testcontainers.LocalStack;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class LocalStackBuilder : ContainerBuilder<LocalStackBuilder, LocalStackContainer, LocalStackConfiguration>
{
    public const string LocalStackImage = "localstack/localstack:2.0";

    public const ushort LocalStackPort = 4566;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackBuilder" /> class.
    /// </summary>
    public LocalStackBuilder()
        : this(new LocalStackConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private LocalStackBuilder(LocalStackConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override LocalStackConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override LocalStackContainer Build()
    {
        Validate();
        return new LocalStackContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Init()
    {
        return base.Init()
            .WithImage(LocalStackImage)
            .WithPortBinding(LocalStackPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/_localstack/health").ForPort(LocalStackPort)));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Merge(LocalStackConfiguration oldValue, LocalStackConfiguration newValue)
    {
        return new LocalStackBuilder(new LocalStackConfiguration(oldValue, newValue));
    }
}