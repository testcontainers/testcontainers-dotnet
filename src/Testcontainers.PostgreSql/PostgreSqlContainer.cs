namespace Testcontainers.PostgreSql;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class PostgreSqlContainer : DockerContainer, IDatabaseContainer
{
    private readonly PostgreSqlConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public PostgreSqlContainer(PostgreSqlConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the PostgreSql connection string.
    /// </summary>
    /// <returns>The PostgreSql connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Host", Hostname);
        properties.Add("Port", GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort).ToString());
        properties.Add("Database", _configuration.Database);
        properties.Add("Username", _configuration.Username);
        properties.Add("Password", _configuration.Password);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Executes the SQL script in the PostgreSql container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "psql", "--username", _configuration.Username, "--dbname", _configuration.Database, "--file", scriptFilePath }, ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Test whether the database is ready to accept connections or not with the <a href="https://www.postgresql.org/docs/current/app-pg-isready.html">pg_isready</a> command.
    /// </summary>
    /// <returns><see langword="true"/> if the database is ready to accept connections; <see langword="false"/> if the database is not yet ready.</returns>
    internal async Task<bool> IsReadyAsync()
    {
        var command = new[] {
            "pg_isready",
            "--host", "localhost", // Explicitly specify localhost in order to be ready only after the initdb scripts have run and the server is listening over TCP/IP
            "--dbname", _configuration.Database,
            "--username", _configuration.Username,
        };
        var result = await ExecAsync(command);
        return result.ExitCode == 0;
    }
}