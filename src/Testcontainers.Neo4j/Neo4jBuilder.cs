namespace Testcontainers.Neo4j;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class Neo4jBuilder : ContainerBuilder<Neo4jBuilder, Neo4jContainer, Neo4jConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    public Neo4jBuilder()
        : this(new Neo4jConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "Neo4jBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "Neo4jBuilder WithNeo4jConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable Neo4jConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private Neo4jBuilder(Neo4jConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override Neo4jConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Neo4j config.
    // /// </summary>
    // /// <param name="config">The Neo4j config.</param>
    // /// <returns>A configured instance of <see cref="Neo4jBuilder" />.</returns>
    // public Neo4jBuilder WithNeo4jConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in Neo4jConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new Neo4jConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override Neo4jContainer Build()
    {
        Validate();
        return new Neo4jContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override Neo4jBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Neo4jConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Neo4jBuilder Merge(Neo4jConfiguration oldValue, Neo4jConfiguration newValue)
    {
        return new Neo4jBuilder(new Neo4jConfiguration(oldValue, newValue));
    }
}