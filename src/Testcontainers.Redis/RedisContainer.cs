namespace Testcontainers.Redis;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class RedisContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedisContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public RedisContainer(RedisConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}