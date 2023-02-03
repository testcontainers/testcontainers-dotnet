namespace Testcontainers.MsSql;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MsSqlBuilder : ContainerBuilder<MsSqlBuilder, MsSqlContainer, MsSqlConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    public MsSqlBuilder()
        : this(new MsSqlConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "MsSqlBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "MsSqlBuilder WithMsSqlConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable MsSqlConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MsSqlBuilder(MsSqlConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override MsSqlConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the MsSql config.
    // /// </summary>
    // /// <param name="config">The MsSql config.</param>
    // /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    // public MsSqlBuilder WithMsSqlConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in MsSqlConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new MsSqlConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override MsSqlContainer Build()
    {
        Validate();
        return new MsSqlContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override MsSqlBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override MsSqlBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Merge(MsSqlConfiguration oldValue, MsSqlConfiguration newValue)
    {
        return new MsSqlBuilder(new MsSqlConfiguration(oldValue, newValue));
    }
}