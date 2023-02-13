namespace Testcontainers.CouchDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class CouchDbBuilder : ContainerBuilder<CouchDbBuilder, CouchDbContainer, CouchDbConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbBuilder" /> class.
    /// </summary>
    public CouchDbBuilder()
        : this(new CouchDbConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "CouchDbBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "CouchDbBuilder WithCouchDbConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable CouchDbConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private CouchDbBuilder(CouchDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override CouchDbConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the CouchDb config.
    // /// </summary>
    // /// <param name="config">The CouchDb config.</param>
    // /// <returns>A configured instance of <see cref="CouchDbBuilder" />.</returns>
    // public CouchDbBuilder WithCouchDbConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in CouchDbConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new CouchDbConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override CouchDbContainer Build()
    {
        Validate();
        return new CouchDbContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override CouchDbBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override CouchDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new CouchDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override CouchDbBuilder Merge(CouchDbConfiguration oldValue, CouchDbConfiguration newValue)
    {
        return new CouchDbBuilder(new CouchDbConfiguration(oldValue, newValue));
    }
}