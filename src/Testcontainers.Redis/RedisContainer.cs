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

    /// <summary>
    /// Gets the Redis connection string.
    /// </summary>
    /// <returns>The Redis connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder("redis", Hostname, GetMappedPublicPort(RedisBuilder.RedisPort)).Uri.Authority;
    }

    /// <summary>
    /// Executes the Lua script in the Redis container.
    /// </summary>
    /// <param name="scriptContent">The content of the Lua script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Lua script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "redis-cli", "--eval", scriptFilePath, "0" }, ct)
            .ConfigureAwait(false);
    }
}