namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DockerComposeBuilder : ContainerBuilder<DockerComposeBuilder, DockerComposeContainer, DockerComposeConfiguration>
{
    public const string DockerComposeFilePath = "/docker-compose.yml";

    public const string DockerComposeImage = "docker:25.0-cli";

    // TODO: This does not support all container runtimes (host configurations). We should do something similar to what we are doing in the Resource Reaper implementation.
    private const string DockerSocketPath = "/var/run/docker.sock";

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeBuilder" /> class.
    /// </summary>
    public DockerComposeBuilder()
        : this(new DockerComposeConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerComposeBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private DockerComposeBuilder(DockerComposeConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override DockerComposeConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Docker Compose file.
    /// </summary>
    /// <param name="composeFilePath">The Docker Compose file path.</param>
    /// <returns>A configured instance of <see cref="DockerComposeBuilder" />.</returns>
    public DockerComposeBuilder WithComposeFile(string composeFilePath)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(composeFilePath: composeFilePath))
            .WithEntrypoint("docker", "compose", "--project-name", Guid.NewGuid().ToString("D"), "--file", composeFilePath)
            .WithResourceMapping(new FileInfo(composeFilePath), new FileInfo(DockerComposeFilePath));
    }

    /// <summary>
    /// Sets the Docker Compose mode.
    /// </summary>
    /// <param name="mode">The Docker Compose mode.</param>
    /// <returns>A configured instance of <see cref="DockerComposeBuilder" />.</returns>
    public DockerComposeBuilder WithComposeMode(DockerComposeMode mode)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(mode: mode));
    }

    /// <inheritdoc />
    public override DockerComposeContainer Build()
    {
        Validate();

        switch (DockerResourceConfiguration.Mode)
        {
            case DockerComposeMode.Local:
                return new DockerComposeLocalContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
            case DockerComposeMode.Remote:
                return new DockerComposeRemoteContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
            default:
                throw new ArgumentOutOfRangeException(nameof(DockerResourceConfiguration.Mode), "Docker Compose mode not supported.");
        }
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Init()
    {
        return base.Init()
            .WithImage(DockerComposeImage)
            .WithCommand("up")
            .WithCommand("--detach")
            .WithComposeMode(DockerComposeMode.Remote)
            .WithBindMount(DockerSocketPath, DockerSocketPath, AccessMode.ReadOnly);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        const string dockerComposeFileNotFound = "Docker Compose file not found.";
        _ = Guard.Argument(DockerResourceConfiguration.ComposeFilePath, nameof(DockerComposeConfiguration.ComposeFilePath))
            .NotNull()
            .NotEmpty()
            .ThrowIf(argument => !File.Exists(argument.Value), argument => new FileNotFoundException(dockerComposeFileNotFound, argument.Value));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Merge(DockerComposeConfiguration oldValue, DockerComposeConfiguration newValue)
    {
        return new DockerComposeBuilder(new DockerComposeConfiguration(oldValue, newValue));
    }
}