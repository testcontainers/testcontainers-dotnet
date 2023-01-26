namespace Testcontainers.ModuleName;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ModuleNameBuilder : ContainerBuilder<ModuleNameBuilder, ModuleNameContainer, ModuleNameConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameBuilder" /> class.
    /// </summary>
    public ModuleNameBuilder()
        : this(new ModuleNameConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "ModuleNameBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "ModuleNameBuilder WithModuleNameConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable ModuleNameConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleNameBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ModuleNameBuilder(ModuleNameConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override ModuleNameConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the ModuleName config.
    // /// </summary>
    // /// <param name="config">The ModuleName config.</param>
    // /// <returns>A configured instance of <see cref="ModuleNameBuilder" />.</returns>
    // public ModuleNameBuilder WithModuleNameConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in ModuleNameConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new ModuleNameConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override ModuleNameContainer Build()
    {
        Validate();
        return new ModuleNameContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override ModuleNameBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override ModuleNameBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ModuleNameConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ModuleNameBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ModuleNameConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ModuleNameBuilder Merge(ModuleNameConfiguration oldValue, ModuleNameConfiguration newValue)
    {
        return new ModuleNameBuilder(new ModuleNameConfiguration(oldValue, newValue));
    }
}