namespace Testcontainers.Redis;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RedisBuilder : ContainerBuilder<RedisBuilder, RedisContainer, RedisConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    public RedisBuilder()
        : this(new RedisConfiguration())
    {
        // 1) To change the ContainerBuilder default configuration override the DockerResourceConfiguration property and the "RedisBuilder Init()" method.
        //    Append the module configuration to base.Init() e.g. base.Init().WithImage("alpine:3.17") to set the modules' default image.

        // 2) To customize the ContainerBuilder validation override the "void Validate()" method.
        //    Use Testcontainers' Guard.Argument<TType>(TType, string) or your own guard implementation to validate the module configuration.

        // 3) Add custom builder methods to extend the ContainerBuilder capabilities such as "RedisBuilder WithRedisConfig(object)".
        //    Merge the current module configuration with a new instance of the immutable RedisConfiguration type to update the module configuration.

        // DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RedisBuilder(RedisConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // DockerResourceConfiguration = resourceConfiguration;
    }

    // /// <inheritdoc />
    // protected override RedisConfiguration DockerResourceConfiguration { get; }

    // /// <summary>
    // /// Sets the Redis config.
    // /// </summary>
    // /// <param name="config">The Redis config.</param>
    // /// <returns>A configured instance of <see cref="RedisBuilder" />.</returns>
    // public RedisBuilder WithRedisConfig(object config)
    // {
    //     // Extends the ContainerBuilder capabilities and holds a custom configuration in RedisConfiguration.
    //     // In case of a module requires other properties to represent itself, extend ContainerConfiguration.
    //     return Merge(DockerResourceConfiguration, new RedisConfiguration(config: config));
    // }

    /// <inheritdoc />
    public override RedisContainer Build()
    {
        Validate();
        return new RedisContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    // /// <inheritdoc />
    // protected override RedisBuilder Init()
    // {
    //     return base.Init();
    // }

    // /// <inheritdoc />
    // protected override void Validate()
    // {
    //     base.Validate();
    // }

    /// <inheritdoc />
    protected override RedisBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RedisConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RedisBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new RedisConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override RedisBuilder Merge(RedisConfiguration oldValue, RedisConfiguration newValue)
    {
        return new RedisBuilder(new RedisConfiguration(oldValue, newValue));
    }
}