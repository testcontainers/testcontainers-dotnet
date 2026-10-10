namespace Testcontainers.RustFs;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RustFsBuilder : ContainerBuilder<RustFsBuilder, RustFsContainer, RustFsConfiguration>
{
    public const ushort RustFsPort = 9000;

    public const string DefaultUsername = "AKIAIOSFODNN7EXAMPLE";

    public const string DefaultPassword = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>rustfs/rustfs:1.0.1</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/rustfs/rustfs/tags" />.
    /// </remarks>
    public RustFsBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/rustfs/rustfs/tags" />.
    /// </remarks>
    public RustFsBuilder(IImage image)
        : this(new RustFsConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RustFsBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RustFsBuilder(RustFsConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override RustFsConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the RustFS username.
    /// </summary>
    /// <param name="username">The RustFS username.</param>
    /// <returns>A configured instance of <see cref="RustFsBuilder" />.</returns>
    public RustFsBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new RustFsConfiguration(username: username))
            .WithEnvironment("RUSTFS_ACCESS_KEY", username);
    }

    /// <summary>
    /// Sets the RustFS password.
    /// </summary>
    /// <param name="password">The RustFS password.</param>
    /// <returns>A configured instance of <see cref="RustFsBuilder" />.</returns>
    public RustFsBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new RustFsConfiguration(password: password))
            .WithEnvironment("RUSTFS_SECRET_KEY", password);
    }

    /// <inheritdoc />
    public override RustFsContainer Build()
    {
        Validate();
        return new RustFsContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override RustFsBuilder Init()
    {
        return base.Init()
            .WithPortBinding(RustFsPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithConnectionStringProvider(new RustFsConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/health/ready").ForPort(RustFsPort)));
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
    protected override RustFsBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RustFsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RustFsBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RustFsConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RustFsBuilder Merge(RustFsConfiguration oldValue, RustFsConfiguration newValue)
    {
        return new RustFsBuilder(new RustFsConfiguration(oldValue, newValue));
    }
}