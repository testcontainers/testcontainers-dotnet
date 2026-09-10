namespace Testcontainers.DuckDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DuckDbContainer : DockerContainer
{
    private readonly DuckDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="DuckDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public DuckDbContainer(DuckDbConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the path of the DuckDB database file inside the container.
    /// </summary>
    /// <remarks>
    /// DuckDB is an embedded database. The database file is created on first use;
    /// copy it out of the container with <see cref="DockerContainer.ReadFileAsync" />
    /// to use it with a DuckDB client library on the test host.
    /// </remarks>
    /// <returns>The DuckDB database file path.</returns>
    public string GetDatabaseFilePath()
    {
        return _configuration.Database;
    }

    /// <summary>
    /// Executes the SQL script in the DuckDB container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, fileMode: Unix.FileMode644, ct: ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { DuckDbBuilder.DuckDbBinaryFilePath, _configuration.Database, "-f", scriptFilePath }, ct)
            .ConfigureAwait(false);
    }
}
