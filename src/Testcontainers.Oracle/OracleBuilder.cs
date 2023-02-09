namespace Testcontainers.Oracle;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OracleBuilder : ContainerBuilder<OracleBuilder, OracleContainer, OracleConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OracleBuilder" /> class.
    /// </summary>
    public OracleBuilder()
        : this(new OracleConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "OracleBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "OracleBuilder WithOracleConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable OracleConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private OracleBuilder(OracleConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override OracleConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Oracle config.
    // /// </summary>
    // /// <param name="config">The Oracle config.</param>
    // /// <returns>A configured instance of <see cref="OracleBuilder" />.</returns>
    // public OracleBuilder WithOracleConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in OracleConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new OracleConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override OracleContainer Build()
    {
        Validate();
        return new OracleContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override OracleBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override OracleBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OracleBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OracleBuilder Merge(OracleConfiguration oldValue, OracleConfiguration newValue)
    {
        return new OracleBuilder(new OracleConfiguration(oldValue, newValue));
    }
}