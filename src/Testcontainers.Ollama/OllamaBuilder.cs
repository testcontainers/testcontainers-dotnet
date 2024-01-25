namespace Testcontainers.Ollama;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OllamaBuilder : ContainerBuilder<OllamaBuilder, OllamaContainer, OllamaConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaBuilder" /> class.
    /// </summary>
    public OllamaBuilder()
        : this(new OllamaConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "Testcontainers.OllamaBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "Testcontainers.OllamaBuilder WithTestcontainers.OllamaConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable Testcontainers.OllamaConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private OllamaBuilder(OllamaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override Testcontainers.OllamaConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Testcontainers.Ollama config.
    // /// </summary>
    // /// <param name="config">The Testcontainers.Ollama config.</param>
    // /// <returns>A configured instance of <see cref="Testcontainers.OllamaBuilder" />.</returns>
    // public Testcontainers.OllamaBuilder WithTestcontainers.OllamaConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in Testcontainers.OllamaConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new Testcontainers.OllamaConfiguration(config: config));
    // }

    /// <inheritdoc />
    protected override OllamaConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override OllamaContainer Build()
    {
        Validate();
        return new OllamaContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override Testcontainers.OllamaBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

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