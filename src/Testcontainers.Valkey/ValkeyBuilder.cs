namespace Testcontainers.Valkey;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ValkeyBuilder : ContainerBuilder<ValkeyBuilder, ValkeyContainer, ValkeyConfiguration>
{
    public const string ValkeyImage = "valkey/valkey:8.1";

    public const ushort ValkeyPort = 6379;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyBuilder" /> class.
    /// </summary>
    public ValkeyBuilder()
        : this(new ValkeyConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ValkeyBuilder(ValkeyConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ValkeyConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override ValkeyContainer Build()
    {
        Validate();
        return new ValkeyContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override ValkeyBuilder Init()
    {
        return base.Init()
            .WithImage(ValkeyImage)
            .WithPortBinding(ValkeyPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Ready to accept connections"));
    }

    /// <inheritdoc />
    protected override ValkeyBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ValkeyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ValkeyBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ValkeyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ValkeyBuilder Merge(ValkeyConfiguration oldValue, ValkeyConfiguration newValue)
    {
        return new ValkeyBuilder(new ValkeyConfiguration(oldValue, newValue));
    }
}