namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class
    DockerComposeBuilder : ContainerBuilder<DockerComposeBuilder, DockerComposeContainer, DockerComposeConfiguration>
{
    private const string NoComposeFile = "No docker compose file have been provided.";

    //Docker Compose is included as part of this image.
    private const string DockerComposeImage = "docker:24-cli";
    private const string DockerSocketPath = "/var/run/docker.sock";
    private const string DockerComposeStartCommand = "docker compose up";

    /// <summary>
    ///     Initializes a new instance of the <see cref="DockerComposeBuilder" /> class.
    /// </summary>
    public DockerComposeBuilder()
        : this(new DockerComposeConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DockerComposeBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private DockerComposeBuilder(DockerComposeConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override DockerComposeConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override DockerComposeContainer Build()
    {
        Validate();

        return new DockerComposeContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <summary>
    ///     Sets the compose file.
    /// </summary>
    /// <param name="composeFile">The compose file.</param>
    /// <returns>A configured instance of <see cref="DockerComposeBuilder" />.</returns>
    public DockerComposeBuilder WithComposeFile(string composeFile)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(composeFile))
            .WithResourceMapping(new FileInfo(composeFile), 
                new FileInfo(DockerComposeCommandLineBuilder.DockerComposeFileName));
    }

    /// <summary>
    ///     If true use a local Docker Compose binary instead of a container.
    /// </summary>
    /// <param name="localCompose">Whether the local compose will be used.</param>
    /// <returns>A configured instance of <see cref="DockerComposeBuilder" />.</returns>
    public DockerComposeBuilder WithLocalCompose(bool localCompose)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration
            (localCompose: localCompose));
    }

    /// <summary>
    ///     Adds options to the docker-compose command, e.g. docker-compose --compatibility.
    /// </summary>
    /// <param name="options">Options for the docker-compose command.</param>
    /// <returns>A configured instance of <see cref="DockerComposeBuilder" />.</returns>
    public DockerComposeBuilder WithOptions(params string[] options)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(options: options))
            .WithCommand(options);
    }

    /// <summary>
    ///     Remove images after containers shutdown.
    /// </summary>
    /// <param name="removeImages"></param>
    /// <returns>A configured instance of <see cref="DockerComposeBuilder" />.</returns>
    public DockerComposeBuilder WithRemoveImages(RemoveImages removeImages)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(removeImages: removeImages));
    }
        
    /// <inheritdoc />
    protected override DockerComposeBuilder Init()
    {
        return base.Init()
            .WithImage(DockerComposeImage)
            .WithEntrypoint(DockerComposeCommandLineBuilder.DockerAppName)
            .WithCommand(DockerComposeCommandLineBuilder
                .FromRemoteConfiguration(DockerResourceConfiguration)
                .BuildStartCommand()
                .ToArray())
            .WithBindMount(DockerSocketPath, DockerSocketPath, AccessMode.ReadOnly);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.ComposeFile, nameof(DockerResourceConfiguration.ComposeFile))
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.ComposeFile, nameof(DockerResourceConfiguration.ComposeFile))
            .ThrowIf(argument => !File.Exists(argument.Value),
                argument => new FileNotFoundException(NoComposeFile, argument.Name));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Clone(
        IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override DockerComposeBuilder Merge(DockerComposeConfiguration oldValue,
        DockerComposeConfiguration newValue)
    {
        return new DockerComposeBuilder(new DockerComposeConfiguration(oldValue, newValue));
    }
}