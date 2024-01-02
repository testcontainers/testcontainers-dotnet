namespace Testcontainers.DockerCompose;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class DockerComposeBuilder : ContainerBuilder<DockerComposeBuilder, DockerComposeContainer, DockerComposeConfiguration>
{
    private const string NoComposeFile = "No docker compose file have been provided.";
    
    //Docker Compose is included as part of this image.
    public const string DockerComposeImage = "docker:24-cli";
    
    public const string DockerSocketPath = "/var/run/docker.sock";
    
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
    
    /// <inheritdoc />
    public override DockerComposeContainer Build()
    {
        Validate();
        
        return new DockerComposeContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }
    
    /// <summary>
    /// Sets the compose file.
    /// </summary>
    /// <param name="composeFile">The compose file.</param>
    /// <returns></returns>
    public DockerComposeBuilder WithComposeFile(string composeFile)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration
            (composeFile: composeFile));
    }
    
    /// <summary>
    /// If true use a local Docker Compose binary instead of a container.
    /// </summary>
    /// <param name="localCompose"></param>
    /// <returns></returns>
    public DockerComposeBuilder WithLocalCompose(bool localCompose)
    {
        return Merge(DockerResourceConfiguration, new DockerComposeConfiguration
        (localCompose: localCompose));
    }
    
    /// <inheritdoc />
    protected override DockerComposeBuilder Init()
    {
        return base.Init()
            .WithImage(DockerComposeImage)
            .WithEntrypoint(CommonCommands.SleepInfinity)
            .WithBindMount(DockerSocketPath, DockerSocketPath, AccessMode.ReadWrite)
            .WithStartupCallback(ConfigureDockerComposeAsync);
    }
    
    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.ComposeFile, nameof(DockerResourceConfiguration.ComposeFile))
            .NotEmpty();
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
    
    /// <summary>
    /// Configures the compose container.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="ct">Cancellation token.</param>
    private async Task ConfigureDockerComposeAsync(IContainer container, CancellationToken ct = default)
    {
        if (container is DockerComposeRemote dockerComposeContainer && 
            !dockerComposeContainer.RuntimeConfiguration.LocalCompose )
        {
            var fileInfo = new FileInfo(dockerComposeContainer.RuntimeConfiguration.ComposeFile);
            if (fileInfo.Exists)
            {
                await container.CopyAsync(fileInfo, ".", Unix.FileMode644, ct)
                    .ConfigureAwait(false);
            }
            await container.ExecAsync(new[] { "docker", "compose", "up",  "-d" }, ct)
                .ConfigureAwait(false);
        }
    }
}