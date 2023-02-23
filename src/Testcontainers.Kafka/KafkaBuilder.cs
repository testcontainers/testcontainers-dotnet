namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class KafkaBuilder : ContainerBuilder<KafkaBuilder, KafkaContainer, KafkaConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    public KafkaBuilder()
        : this(new KafkaConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "KafkaBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "KafkaBuilder WithKafkaConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable KafkaConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private KafkaBuilder(KafkaConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override KafkaConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Kafka config.
    // /// </summary>
    // /// <param name="config">The Kafka config.</param>
    // /// <returns>A configured instance of <see cref="KafkaBuilder" />.</returns>
    // public KafkaBuilder WithKafkaConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in KafkaConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new KafkaConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override KafkaContainer Build()
    {
        Validate();
        return new KafkaContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override KafkaBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override KafkaBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KafkaBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new KafkaConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override KafkaBuilder Merge(KafkaConfiguration oldValue, KafkaConfiguration newValue)
    {
        return new KafkaBuilder(new KafkaConfiguration(oldValue, newValue));
    }
}