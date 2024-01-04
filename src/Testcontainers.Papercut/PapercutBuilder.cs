namespace Testcontainers.Papercut;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PapercutBuilder : ContainerBuilder<PapercutBuilder, PapercutContainer, PapercutConfiguration>
{
    public const string PapercutImage = "jijiechen/papercut:latest";

    public const ushort HttpPort = 37408;

    public const ushort SmtpPort = 25;

    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutBuilder" /> class.
    /// </summary>
    public PapercutBuilder()
        : this(new PapercutConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PapercutBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PapercutBuilder(PapercutConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PapercutConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override PapercutContainer Build()
    {
        Validate();
        return new PapercutContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override PapercutBuilder Init()
    {
        return base.Init()
            .WithImage(PapercutImage)
            .WithPortBinding(HttpPort, true)
            .WithPortBinding(SmtpPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Now listening on"));
    }

    /// <inheritdoc />
    protected override PapercutBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PapercutConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PapercutBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PapercutConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PapercutBuilder Merge(PapercutConfiguration oldValue, PapercutConfiguration newValue)
    {
        return new PapercutBuilder(new PapercutConfiguration(oldValue, newValue));
    }
}