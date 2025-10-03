namespace Testcontainers.DragonflyDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DragonflyDbContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public DragonflyDbContainer(DragonflyDbConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the DragonflyDb connection string.
    /// </summary>
    /// <returns>The DragonflyDb connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder("DragonflyDb", Hostname, GetMappedPublicPort(DragonflyDbBuilder.DragonflyDbPort)).Uri.Authority;
    }

    /// <summary>
    /// Executes the Lua script in the DragonflyDb container.
    /// </summary>
    /// <param name="scriptContent">The content of the Lua script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Lua script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, fileMode: Unix.FileMode644, ct: ct)
            .ConfigureAwait(false);
        
        //DragonflyDb uses the same cli as redis
        return await ExecAsync(new[] { "redis-cli", "--eval", scriptFilePath, "0" }, ct)
            .ConfigureAwait(false);
    }
}