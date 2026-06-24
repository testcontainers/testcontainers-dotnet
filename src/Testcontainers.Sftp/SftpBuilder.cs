namespace Testcontainers.Sftp;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SftpBuilder : ContainerBuilder<SftpBuilder, SftpContainer, SftpConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string SftpImage = "atmoz/sftp:alpine";

    public const ushort SftpPort = 22;

    public const string DefaultUsername = "sftp";

    public const string DefaultPassword = "sftp";

    public const string DefaultUploadDirectory = "upload";

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public SftpBuilder()
        : this(SftpImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>atmoz/sftp:alpine</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/atmoz/sftp/tags" />.
    /// </remarks>
    public SftpBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/atmoz/sftp/tags" />.
    /// </remarks>
    public SftpBuilder(IImage image)
        : this(new SftpConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SftpBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SftpBuilder(SftpConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override SftpConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Sftp username.
    /// </summary>
    /// <param name="username">The Sftp username.</param>
    /// <returns>A configured instance of <see cref="SftpBuilder" />.</returns>
    public SftpBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new SftpConfiguration(username: username));
    }

    /// <summary>
    /// Sets the Sftp password.
    /// </summary>
    /// <param name="password">The Sftp password.</param>
    /// <returns>A configured instance of <see cref="SftpBuilder" />.</returns>
    public SftpBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new SftpConfiguration(password: password));
    }

    /// <summary>
    /// Sets the directory to which files are uploaded.
    /// </summary>
    /// <param name="uploadDirectory">The upload directory.</param>
    /// <returns>A configured instance of <see cref="SftpBuilder" />.</returns>
    public SftpBuilder WithUploadDirectory(string uploadDirectory)
    {
        return Merge(DockerResourceConfiguration, new SftpConfiguration(uploadDirectory: uploadDirectory));
    }

    /// <inheritdoc />
    public override SftpContainer Build()
    {
        Validate();

        var sftpBuilder = WithCommand(string.Join(
            ":",
            DockerResourceConfiguration.Username,
            DockerResourceConfiguration.Password,
            string.Empty,
            string.Empty,
            DockerResourceConfiguration.UploadDirectory));

        return new SftpContainer(sftpBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override SftpBuilder Init()
    {
        return base.Init()
            .WithPortBinding(SftpPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithUploadDirectory(DefaultUploadDirectory)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Server listening on .+"));
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

        _ = Guard.Argument(DockerResourceConfiguration.UploadDirectory, nameof(DockerResourceConfiguration.UploadDirectory))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override SftpBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SftpConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SftpBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SftpConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SftpBuilder Merge(SftpConfiguration oldValue, SftpConfiguration newValue)
    {
        return new SftpBuilder(new SftpConfiguration(oldValue, newValue));
    }
}