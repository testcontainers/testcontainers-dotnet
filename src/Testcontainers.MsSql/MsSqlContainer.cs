namespace Testcontainers.MsSql;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MsSqlContainer : DockerContainer, IDatabaseContainer
{
    private readonly MsSqlConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MsSqlContainer(MsSqlConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MsSql connection string.
    /// </summary>
    /// <returns>The MsSql connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Server", Hostname + "," + GetMappedPublicPort(MsSqlBuilder.MsSqlPort));
        properties.Add("Database", _configuration.Database);
        properties.Add("User Id", _configuration.Username);
        properties.Add("Password", _configuration.Password);
        properties.Add("TrustServerCertificate", bool.TrueString);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Executes the SQL script in the MsSql container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        var hasMsSql18Tools = await ExecAsync(new[] { "[", "-f" , "/opt/mssql-tools18/bin/sqlcmd", "]" }, ct)
            .ConfigureAwait(false);

        var command = new[]
        {
            hasMsSql18Tools.ExitCode == 0 ? "/opt/mssql-tools18/bin/sqlcmd" : "/opt/mssql-tools/bin/sqlcmd",
            "-C",
            "-b",
            "-r",
            "1",
            "-U",
            _configuration.Username,
            "-P",
            _configuration.Password,
            "-i",
            scriptFilePath,
        };
        
        return await ExecAsync(command, ct)
            .ConfigureAwait(false);
    }
}