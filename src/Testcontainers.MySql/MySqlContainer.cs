namespace Testcontainers.MySql;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MySqlContainer : DockerContainer, IDatabaseContainer
{
    private readonly MySqlConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MySqlContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public MySqlContainer(MySqlConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MySql connection string.
    /// </summary>
    /// <returns>The MySql connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Server", Hostname);
        properties.Add("Port", GetMappedPublicPort(MySqlBuilder.MySqlPort).ToString());
        properties.Add("Database", _configuration.Database);
        properties.Add("Uid", _configuration.Username);
        properties.Add("Pwd", _configuration.Password);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Executes the SQL script in the MySql container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "mysql", "--protocol=TCP", $"--port={MySqlBuilder.MySqlPort}", $"--user={_configuration.Username}", $"--password={_configuration.Password}", _configuration.Database, $"--execute=source {scriptFilePath};" }, ct)
            .ConfigureAwait(false);
    }
}