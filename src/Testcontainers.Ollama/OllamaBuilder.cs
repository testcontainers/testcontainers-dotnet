namespace Testcontainers.Ollama;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OllamaBuilder : ContainerBuilder<OllamaBuilder, OllamaContainer, OllamaConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string OllamaImage = "ollama/ollama:0.6.6";

    public const ushort OllamaPort = 11434;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public OllamaBuilder()
        : this(OllamaImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/r/ollama/ollama/tags">https://hub.docker.com/r/ollama/ollama/tags</see>.</param>
    public OllamaBuilder(string image)
        : this(new OllamaConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public OllamaBuilder(IImage image)
        : this(new OllamaConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
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

    /// <inheritdoc />
    public override OllamaContainer Build()
    {
        Validate();
        return new OllamaContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override OllamaBuilder Init()
    {
        return base.Init()
            .WithPortBinding(OllamaPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/api/version").ForPort(OllamaPort)));
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