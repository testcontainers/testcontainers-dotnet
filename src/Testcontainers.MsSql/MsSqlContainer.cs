namespace Testcontainers.MsSql;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MsSqlContainer : DockerContainer, IDatabaseContainer
{
    private static readonly string[] FindSqlCmdFilePath = { "/bin/sh", "-c", "find /opt/mssql-tools*/bin/sqlcmd -type f -print -quit" };

    private readonly Lazy<Task<string>> _lazySqlCmdFilePath;

    private readonly MsSqlConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MsSqlContainer(MsSqlConfiguration configuration)
        : base(configuration)
    {
        _lazySqlCmdFilePath = new Lazy<Task<string>>(FindSqlCmdFilePathAsync);
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
    /// Gets the <c>sqlcmd</c> utility file path.
    /// </summary>
    /// <remarks>
    /// The file path represents the path from the container, not from the Docker or test host.
    /// </remarks>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the <c>sqlcmd</c> utility file path has been found.</returns>
    public Task<string> GetSqlCmdFilePathAsync(CancellationToken ct = default)
    {
        return _lazySqlCmdFilePath.Value;
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

        var sqlCmdFilePath = await GetSqlCmdFilePathAsync(ct)
            .ConfigureAwait(false);

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { sqlCmdFilePath, "-C", "-b", "-r", "1", "-U", _configuration.Username, "-P", _configuration.Password, "-i", scriptFilePath }, ct)
            .ConfigureAwait(false);
    }

    private async Task<string> FindSqlCmdFilePathAsync()
    {
        var findSqlCmdFilePathExecResult = await ExecAsync(FindSqlCmdFilePath)
            .ConfigureAwait(false);

        if (findSqlCmdFilePathExecResult.ExitCode == 0)
        {
            return findSqlCmdFilePathExecResult.Stdout.Trim();
        }

        throw new NotSupportedException("The sqlcmd binary could not be found.");
    }
}