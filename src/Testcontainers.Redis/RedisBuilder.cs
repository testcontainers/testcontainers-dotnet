namespace Testcontainers.Redis;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RedisBuilder : ContainerBuilder<RedisBuilder, RedisContainer, RedisConfiguration>
{
    public const string RedisImage = "redis:7.0";

    public const ushort RedisPort = 6379;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public RedisBuilder()
        : this(RedisImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/_/redis/tags">https://hub.docker.com/_/redis/tags</see>.</param>
    public RedisBuilder(string image)
        : this(new RedisConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public RedisBuilder(IImage image)
        : this(new RedisConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private RedisBuilder(RedisConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override RedisConfiguration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    public override RedisContainer Build()
    {
        Validate();
        return new RedisContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override RedisBuilder Init()
    {
        return base.Init()
            .WithImage(RedisImage)
            .WithPortBinding(RedisPort, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("redis-cli", "ping"));
    }

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