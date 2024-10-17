namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ToxiproxyBuilder : ContainerBuilder<ToxiproxyBuilder, ToxiproxyContainer, ToxiproxyConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    public ToxiproxyBuilder()
        : this(new ToxiproxyConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "ToxiproxyBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "ToxiproxyBuilder WithToxiproxyConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable ToxiproxyConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ToxiproxyBuilder(ToxiproxyConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override ToxiproxyConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Toxiproxy config.
    // /// </summary>
    // /// <param name="config">The Toxiproxy config.</param>
    // /// <returns>A configured instance of <see cref="ToxiproxyBuilder" />.</returns>
    // public ToxiproxyBuilder WithToxiproxyConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in ToxiproxyConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new ToxiproxyConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override ToxiproxyContainer Build()
    {
        Validate();
        return new ToxiproxyContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override ToxiproxyBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ToxiproxyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ToxiproxyConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ToxiproxyBuilder Merge(ToxiproxyConfiguration oldValue, ToxiproxyConfiguration newValue)
    {
        return new ToxiproxyBuilder(new ToxiproxyConfiguration(oldValue, newValue));
    }
}