
namespace Testcontainers.ClamAv;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ClamAvBuilder : ContainerBuilder<ClamAvBuilder, ClamAvContainer, ClamAvConfiguration>
{
    public const string ClamAvImage = "clamav/clamav:1.0.6";

    public const ushort ClamAvPort = 3310;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    public ClamAvBuilder()
        : this(new ClamAvConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ClamAvBuilder(ClamAvConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override ClamAvConfiguration DockerResourceConfiguration { get; }

    public override ClamAvContainer Build()
    {
        Validate();
        return new ClamAvContainer(DockerResourceConfiguration);
    }

    protected override ClamAvBuilder Init()
    {
        return base.Init()
            .WithImage(ClamAvImage)
            .WithPortBinding(ClamAvPort, true)
            .WithEnvironment("CLAMAV_NO_CLAMD", "false")
            .WithEnvironment("CLAMAV_NO_FRESHCLAMD", "true")
            .WithEnvironment("CLAMAV_NO_MILTERD", "true")
            .WithEnvironment("CLAMD_STARTUP_TIMEOUT", "1800")
            .WithEnvironment("FRESHCLAM_CHECKS", "1")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("socket found, clamd started."));
    }

    protected override ClamAvBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ClamAvConfiguration(resourceConfiguration));
    }

    protected override ClamAvBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ClamAvConfiguration(resourceConfiguration));
    }

    protected override ClamAvBuilder Merge(ClamAvConfiguration oldValue, ClamAvConfiguration newValue)
    {
        return new ClamAvBuilder(new ClamAvConfiguration(oldValue, newValue));
    }
}
