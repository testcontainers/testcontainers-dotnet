namespace TestContainers.Minio;

[PublicAPI]
public sealed class MinioBuilder : ContainerBuilder<MinioBuilder, MinioContainer, MinioConfiguration>
{
    public const ushort MinioPort = 9000;
    public const string MinioImage = "minio/minio:RELEASE.2023-01-31T02-24-19Z";
    
    protected override MinioConfiguration DockerResourceConfiguration { get; }
    
    public MinioBuilder(MinioConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
        
    }

    public MinioBuilder()
        : this(new MinioConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }
    
    public MinioBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new MinioConfiguration(userName: username))
            .WithEnvironment("MINIO_ROOT_USER", username);
    }
    
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
            .WithUsername(DockerResourceConfiguration.UserName)
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
        
        _ = Guard.Argument(DockerResourceConfiguration.UserName, DockerResourceConfiguration.UserName).NotNull().NotEmpty();
        _ = Guard.Argument(DockerResourceConfiguration.Password, DockerResourceConfiguration.Password).NotNull().NotEmpty();
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