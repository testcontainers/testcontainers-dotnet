namespace Testcontainers.Ollama;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OllamaBuilder : ContainerBuilder<OllamaBuilder, OllamaContainer, OllamaConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string OllamaImage = "ollama/ollama:0.6.6";

    public const ushort OllamaPort = 11434;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public OllamaBuilder()
        : this(OllamaImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>ollama/ollama:0.6.6</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ollama/ollama/tags" />.
    /// </remarks>
    public OllamaBuilder(string image)
        : this(new OllamaConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ollama/ollama/tags" />.
    /// </remarks>
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