namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class AzuriteBuilder : ContainerBuilder<AzuriteBuilder, AzuriteContainer, AzuriteConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    public AzuriteBuilder()
        : this(new AzuriteConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "AzuriteBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "AzuriteBuilder WithAzuriteConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable AzuriteConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private AzuriteBuilder(AzuriteConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override AzuriteConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Azurite config.
    // /// </summary>
    // /// <param name="config">The Azurite config.</param>
    // /// <returns>A configured instance of <see cref="AzuriteBuilder" />.</returns>
    // public AzuriteBuilder WithAzuriteConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in AzuriteConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new AzuriteConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override AzuriteContainer Build()
    {
        Validate();
        return new AzuriteContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override AzuriteBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new AzuriteConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override AzuriteBuilder Merge(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
    {
        return new AzuriteBuilder(new AzuriteConfiguration(oldValue, newValue));
    }
}