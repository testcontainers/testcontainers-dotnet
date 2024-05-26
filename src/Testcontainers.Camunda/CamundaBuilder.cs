namespace Testcontainers.Camunda;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public class CamundaBuilder : ContainerBuilder<CamundaBuilder, CamundaContainer, CamundaConfiguration>
{
    public const string CamundaImage = "camunda/camunda-bpm-platform:wildfly-7.22.0-SNAPSHOT";
    
    public const ushort CamundaPort = 8080;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaBuilder" /> class.
    /// </summary>
    public CamundaBuilder()
        : this(new CamundaConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CamundaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CamundaBuilder(CamundaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override CamundaConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override CamundaContainer Build()
    {
        Validate();
        return new CamundaContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override CamundaBuilder Init()
    {
        return base.Init()
            .WithImage(CamundaImage)
            .WithPortBinding(CamundaPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("(?s).*Engine created.*$"));
    }

    /// <inheritdoc />
    protected override CamundaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CamundaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CamundaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CamundaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CamundaBuilder Merge(CamundaConfiguration oldValue, CamundaConfiguration newValue)
    {
        return new CamundaBuilder(new CamundaConfiguration(oldValue, newValue));
    }
}