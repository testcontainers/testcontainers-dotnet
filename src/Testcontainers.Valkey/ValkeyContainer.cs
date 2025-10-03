namespace Testcontainers.Valkey;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ValkeyContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ValkeyContainer(ValkeyConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Valkey connection string.
    /// </summary>
    /// <returns>The Valkey connection string.</returns>
    public string GetConnectionString()
    {
        return new UriBuilder("valkey", Hostname, GetMappedPublicPort(ValkeyBuilder.ValkeyPort)).Uri.Authority;
    }

    /// <summary>
    /// Executes the Lua script in the Valkey container.
    /// </summary>
    /// <param name="scriptContent">The content of the Lua script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Lua script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "valkey-cli", "--eval", scriptFilePath, "0" }, ct)
            .ConfigureAwait(false);
    }
}