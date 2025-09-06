namespace Testcontainers.Ollama;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OllamaBuilder
    : ContainerBuilder<OllamaBuilder, OllamaContainer, OllamaConfiguration>
{
    public const string OllamaImage = "ollama/ollama:0.6.6";

    public const ushort OllamaPort = 11434;

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
            .WithImage(OllamaImage)
            .WithPortBinding(OllamaPort, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(request =>
                        request.ForPath("/api/version").ForPort(OllamaPort)
                    )
            );
    }

    /// <inheritdoc />
    protected override OllamaBuilder Clone(
        IResourceConfiguration<CreateContainerParameters> resourceConfiguration
    )
    {
        return Merge(DockerResourceConfiguration, new OllamaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OllamaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OllamaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OllamaBuilder Merge(
        OllamaConfiguration oldValue,
        OllamaConfiguration newValue
    )
    {
        return new OllamaBuilder(new OllamaConfiguration(oldValue, newValue));
    }
}
