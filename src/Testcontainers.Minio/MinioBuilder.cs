namespace Testcontainers.Minio;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MinioBuilder : ContainerBuilder<MinioBuilder, MinioContainer, MinioConfiguration>
{
    public const ushort MinioPort = 9000;
    public const string MinioImage = "minio/minio:RELEASE.2023-01-31T02-24-19Z";
    
    protected override MinioConfiguration DockerResourceConfiguration { get; }


    /// <summary>
    /// Initializes a new instance of the <see cref="MinioBuilder" /> class.
    /// </summary>
    public MinioBuilder()
        : this(new MinioConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="MinioBuilder" /> class.
    /// </summary>
    /// <param name="dockerResourceConfiguration">The Docker resource configuration.</param>
    private MinioBuilder(MinioConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
        
    }
    
    /// <summary>
    /// Sets the Minio username.
    /// </summary>
    /// <param name="username">The Minio username.</param>
    /// <returns>A configured instance of <see cref="MinioBuilder" />.</returns>
    public MinioBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new MinioConfiguration(username: username))
            .WithEnvironment("MINIO_ROOT_USER", username);
    }
    
    /// <summary>
    /// Sets the Minio password.
    /// </summary>
    /// <param name="password">The Minio password.</param>
    /// <returns>A configured instance of <see cref="MinioBuilder" />.</returns>
    public MinioBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new MinioConfiguration(password: password))
            .WithEnvironment("MINIO_ROOT_PASSWORD", password);
    }
    
    protected override MinioBuilder Init()
    {
        return base.Init()
            .WithImage(MinioImage)
            .WithPortBinding(MinioPort, true)
            .WithUsername(DockerResourceConfiguration.Username)
            .WithPassword(DockerResourceConfiguration.Password)
            .WithCommand("server", "/data")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(MinioPort));
    }
    
    
    public override MinioContainer Build()
    {
        Validate();
        return new MinioContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    
    protected override void Validate()
    {
        base.Validate();
        
        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username)).NotNull().NotEmpty();
        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password)).NotNull().NotEmpty();
    }

    protected override MinioBuilder Merge(MinioConfiguration oldValue, MinioConfiguration newValue)
    {
        return new MinioBuilder(new MinioConfiguration(oldValue, newValue));
    }

    protected override MinioBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MinioConfiguration(resourceConfiguration));
    }
    
    protected override MinioBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MinioConfiguration(resourceConfiguration));
    }
}