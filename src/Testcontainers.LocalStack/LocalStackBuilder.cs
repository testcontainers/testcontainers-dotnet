using System.Linq;
using System.Threading.Tasks;

namespace Testcontainers.LocalStack;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class LocalStackBuilder : ContainerBuilder<LocalStackBuilder, LocalStackContainer, LocalStackConfiguration>
{
    public const ushort LocalStackPort = 4566;
    public const string LocalStackImage = "localstack/localstack:1.3.1";
    
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
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private LocalStackBuilder(LocalStackConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override LocalStackConfiguration DockerResourceConfiguration { get; }
    
    /// <summary>
    /// Sets the LocalStack Default Ports Start.
    /// </summary>
    /// <param name="port">The LocalStack Default Ports Start.</param>
    /// <returns>A configured instance of <see cref="LocalStackBuilder" />.</returns>
    public LocalStackBuilder WithExternalServicePortStart(string port)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(externalServicePortStart: port))
            .WithEnvironment("EXTERNAL_SERVICE_PORTS_START", port);
    }
    
    /// <summary>
    /// Sets the LocalStack Default Ports End.
    /// </summary>
    /// <param name="port">The LocalStack Default Ports End.</param>
    /// <returns>A configured instance of <see cref="LocalStackBuilder" />.</returns>
    public LocalStackBuilder WithExternalServicePortEnd(string port)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(externalServicePortEnd: port))
            .WithEnvironment("EXTERNAL_SERVICE_PORTS_END", port);
    }

    /// <inheritdoc />
    public override LocalStackContainer Build()
    {
        Validate();
        return new LocalStackContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    protected override LocalStackBuilder Init()
    {
        return base.Init()
            .WithImage(LocalStackImage)
            .WithPortBinding(LocalStackPort, true)
            .WithExternalServicePortStart("4510")
            .WithExternalServicePortEnd("4559")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/health").ForPort(LocalStackPort)));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.ExternalServicePortStart, nameof(DockerResourceConfiguration.ExternalServicePortStart))
            .NotNull()
            .NotEmpty();
        
        _ = Guard.Argument(DockerResourceConfiguration.ExternalServicePortEnd, nameof(DockerResourceConfiguration.ExternalServicePortEnd))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override LocalStackBuilder Merge(LocalStackConfiguration oldValue, LocalStackConfiguration newValue)
    {
        return new LocalStackBuilder(new LocalStackConfiguration(oldValue, newValue));
    }
}