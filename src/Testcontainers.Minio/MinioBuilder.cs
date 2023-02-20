namespace Testcontainers.Minio;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MinioBuilder : ContainerBuilder<MinioBuilder, MinioContainer, MinioConfiguration>
{
    public const string MinioImage = "minio/minio:RELEASE.2023-01-31T02-24-19Z";

    public const ushort MinioPort = 9000;

    public const string DefaultUsername = "AKIAIOSFODNN7EXAMPLE";

    public const string DefaultPassword = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";

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
    private MinioBuilder(MinioConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override MinioConfiguration DockerResourceConfiguration { get; }

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

    /// <inheritdoc />
    public override MinioContainer Build()
    {
        Validate();
        return new MinioContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override MinioBuilder Init()
    {
        return base.Init()
            .WithImage(MinioImage)
            .WithPortBinding(MinioPort, true)
            .WithCommand("server", "/data")
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/minio/health/ready").ForPort(MinioPort)));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override MinioBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MinioConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MinioBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MinioConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MinioBuilder Merge(MinioConfiguration oldValue, MinioConfiguration newValue)
    {
        return new MinioBuilder(new MinioConfiguration(oldValue, newValue));
    }
}