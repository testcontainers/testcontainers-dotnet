namespace Testcontainers.Redis;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class RedisBuilder : ContainerBuilder<RedisBuilder, RedisContainer, RedisConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string RedisImage = "redis:7.0";

    public const ushort RedisPort = 6379;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public RedisBuilder()
        : this(RedisImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>redis:7.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/redis/tags" />.
    /// </remarks>
    public RedisBuilder(string image)
        : this(new RedisConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/_/redis/tags" />.
    /// </remarks>
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