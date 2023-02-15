namespace Testcontainers.Elasticsearch;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class ElasticsearchBuilder : ContainerBuilder<ElasticsearchBuilder, ElasticsearchContainer, ElasticsearchConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    public ElasticsearchBuilder()
        : this(new ElasticsearchConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "ElasticsearchBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "ElasticsearchBuilder WithElasticsearchConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable ElasticsearchConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElasticsearchBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private ElasticsearchBuilder(ElasticsearchConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override ElasticsearchConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Elasticsearch config.
    // /// </summary>
    // /// <param name="config">The Elasticsearch config.</param>
    // /// <returns>A configured instance of <see cref="ElasticsearchBuilder" />.</returns>
    // public ElasticsearchBuilder WithElasticsearchConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in ElasticsearchConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new ElasticsearchConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override ElasticsearchContainer Build()
    {
        Validate();
        return new ElasticsearchContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override ElasticsearchBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ElasticsearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new ElasticsearchConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override ElasticsearchBuilder Merge(ElasticsearchConfiguration oldValue, ElasticsearchConfiguration newValue)
    {
        return new ElasticsearchBuilder(new ElasticsearchConfiguration(oldValue, newValue));
    }
}