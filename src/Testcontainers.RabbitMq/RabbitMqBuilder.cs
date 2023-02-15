namespace Testcontainers.RabbitMq;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RabbitMqBuilder : ContainerBuilder<RabbitMqBuilder, RabbitMqContainer, RabbitMqConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqBuilder" /> class.
    /// </summary>
    public RabbitMqBuilder()
        : this(new RabbitMqConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "RabbitMqBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "RabbitMqBuilder WithRabbitMqConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable RabbitMqConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RabbitMqBuilder(RabbitMqConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override RabbitMqConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the RabbitMq config.
    // /// </summary>
    // /// <param name="config">The RabbitMq config.</param>
    // /// <returns>A configured instance of <see cref="RabbitMqBuilder" />.</returns>
    // public RabbitMqBuilder WithRabbitMqConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in RabbitMqConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new RabbitMqConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override RabbitMqContainer Build()
    {
        Validate();
        return new RabbitMqContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override RabbitMqBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override RabbitMqBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RabbitMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RabbitMqBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RabbitMqConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RabbitMqBuilder Merge(RabbitMqConfiguration oldValue, RabbitMqConfiguration newValue)
    {
        return new RabbitMqBuilder(new RabbitMqConfiguration(oldValue, newValue));
    }
}