using System.Linq;
using System.Threading.Tasks;

namespace Testcontainers.Minio;

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
    /// Sets the LocalStack default region.
    /// </summary>
    /// <param name="defaultRegion">The LocalStack Default Region.</param>
    /// <returns>A configured instance of <see cref="LocalStackBuilder" />.</returns>
    public LocalStackBuilder WithDefaultRegion(string defaultRegion)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(defaultRegion: defaultRegion))
            .WithEnvironment("DEFAULT_REGION", defaultRegion);
    }
    
    /// <summary>
    /// Sets the Minio username.
    /// </summary>
    /// <param name="port">The LocalStack Default Ports Start.</param>
    /// <returns>A configured instance of <see cref="LocalStackBuilder" />.</returns>
    public LocalStackBuilder WithExternalServicePortStart(string port)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(externalServicePortStart: port))
            .WithEnvironment("EXTERNAL_SERVICE_PORTS_START", port);
    }
    
    /// <summary>
    /// Sets the Minio username.
    /// </summary>
    /// <param name="port">The LocalStack Default Ports End.</param>
    /// <returns>A configured instance of <see cref="LocalStackBuilder" />.</returns>
    public LocalStackBuilder WithExternalServicePortEnd(string port)
    {
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(externalServicePortEnd: port))
            .WithEnvironment("EXTERNAL_SERVICE_PORTS_END", port);
    }
    
    /// <summary>
    /// Sets the Minio username.
    /// </summary>
    /// <param name="services">The LocalStack services.</param>
    /// <returns>A configured instance of <see cref="LocalStackBuilder" />.</returns>
    public LocalStackBuilder WithServices(params AwsService[] services)
    {
        if (services?.Length is null)
        {
            return this;
        }

        string servicesNames = string.Join("SERVICES", services.Select(service => service.Name));
        return Merge(DockerResourceConfiguration, new LocalStackConfiguration(services: services))
            .WithEnvironment("SERVICES", servicesNames);
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
            .WithExternalServicePortStart("4510")
            .WithExternalServicePortEnd("4559")
            .WithEnvironment("USE_SSL", "false")
            .WithDefaultRegion("eu-west-1")
            .WithServices()
            .WithPortBinding(LocalStackPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new UntilReady()));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.UseSsl, nameof(DockerResourceConfiguration.UseSsl))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.ExternalServicePortStart, nameof(DockerResourceConfiguration.ExternalServicePortStart))
            .NotNull()
            .NotEmpty();
        
        _ = Guard.Argument(DockerResourceConfiguration.ExternalServicePortEnd, nameof(DockerResourceConfiguration.ExternalServicePortEnd))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.UseSsl, nameof(DockerResourceConfiguration.UseSsl))
            .NotNull()
            .NotEmpty();
        
        _ = Guard.Argument(DockerResourceConfiguration.Services, nameof(DockerResourceConfiguration.Services))
            .NotNull();
        
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
    
    private sealed class UntilReady : IWaitUntil
    {
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, _) = await container.GetLogs()
                .ConfigureAwait(false);
            return stdout != null && stdout.Contains("Ready.\n");
        }
    }
}