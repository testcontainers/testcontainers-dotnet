namespace Testcontainers.Redis;

/// <summary>
/// Provides the Redis connection string.
/// </summary>
internal sealed class RedisConnectionStringProvider : ContainerConnectionStringProvider<RedisContainer, RedisConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}