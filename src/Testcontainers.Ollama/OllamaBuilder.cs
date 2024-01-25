namespace Testcontainers.Ollama;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OllamaBuilder : ContainerBuilder<OllamaBuilder, OllamaContainer, OllamaConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    public OllamaBuilder()
        : this(new OllamaConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private OllamaBuilder(OllamaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override OllamaConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Testcontainers.Ollama config.
    /// </summary>
    /// <param name="config">The Testcontainers.Ollama config.</param>
    /// <returns>A configured instance of <see cref="OllamaBuilder" />.</returns>
    public OllamaBuilder OllamaConfig(OllamaConfiguration config)
    {
        return Merge(DockerResourceConfiguration, config);
    }

    /// <inheritdoc />
    public override OllamaContainer Build()
    {
        Validate();
        return new OllamaContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        Guard.Argument(DockerResourceConfiguration.Port, nameof(DockerResourceConfiguration.Port)).ThrowIf(info => info.Value is < 1 or > 65535, info => new ArgumentOutOfRangeException(info.Name, info.Value, $"The port must be between 1 and 65535."));
        Guard.Argument(DockerResourceConfiguration.ModelName, nameof(DockerResourceConfiguration.ModelName)).NotNull().NotEmpty();
        Guard.Argument(DockerResourceConfiguration.ImageName, nameof(DockerResourceConfiguration.ImageName)).NotNull().NotEmpty();
        Guard.Argument(DockerResourceConfiguration.HostName, nameof(DockerResourceConfiguration.HostName)).NotNull().NotEmpty();
        Guard.Argument(DockerResourceConfiguration.Schema, nameof(DockerResourceConfiguration.Schema)).NotNull().NotEmpty();
        
        base.Validate();
    }

    /// <inheritdoc />
    protected override OllamaBuilder Init()
    {
        return base.Init()
                .WithName("ollama-container")
                .WithImage(new DockerImage(DockerResourceConfiguration.ImageName))
                .WithHostname(DockerResourceConfiguration.HostName)
                .WithPortBinding(DockerResourceConfiguration.Port, DockerResourceConfiguration.Port)
                .WithVolumeMount("ollama", "/root/.ollama")
                .WithExposedPort(DockerResourceConfiguration.Port)
            ;
    }

    /// <inheritdoc />
    protected override OllamaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OllamaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OllamaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OllamaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OllamaBuilder Merge(OllamaConfiguration oldValue, OllamaConfiguration newValue)
    {
        return new OllamaBuilder(new OllamaConfiguration(oldValue, newValue));
    }
}