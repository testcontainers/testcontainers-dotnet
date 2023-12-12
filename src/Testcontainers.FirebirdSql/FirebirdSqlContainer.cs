namespace Testcontainers.FirebirdSql;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public class FirebirdSqlContainer : DockerContainer, IDatabaseContainer
{
    public const string TestQueryString = "select 1 from RDB$DATABASE;";

    private readonly FirebirdSqlConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public FirebirdSqlContainer(FirebirdSqlConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        this._configuration = configuration;
    }

    private string GetDatabaseName()
    {
        if (State == TestcontainersStates.Running && IsFirebird2_5Image)
        {
            var dbPath = _configuration.Environments.TryGetValue("DBPATH", out var path)
                ? path
                : "/firebird/data";
            return $"{dbPath}/{_configuration.Database}";
        }
        else
        {
            return _configuration.Database;
        }
    }

    /// <summary>
    /// Indicates whether the used image is an EOL v2.5 version of firebird
    /// </summary>
    /// <returns>True if the image is a v2.5 version, false otherwise</returns>
    public bool IsFirebird2_5Image => Image.Tag.StartsWith("2.5") || Image.Tag.StartsWith("v2.5");

    /// <summary>
    /// Gets the FirebirdSql connection string.
    /// </summary>
    /// <returns>The FirebirdSql connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("User", _configuration.Username);
        properties.Add("Password", _configuration.Username == FirebirdSqlBuilder.FirebirdSysdba
            ? _configuration.SysdbaPassword
            : _configuration.Password);
        properties.Add("Database", GetDatabaseName());
        properties.Add("DataSource", $"{Hostname}");
        properties.Add("Port", $"{GetMappedPublicPort(FirebirdSqlBuilder.FirebirdSqlPort)}");
        properties.Add("Dialect", "3");
        properties.Add("Charset", "utf8");
        properties.Add("Role", "");
        properties.Add("Connection lifetime", "15");
        properties.Add("Pooling", "true");
        properties.Add("MinPoolSize", "0");
        properties.Add("MaxPoolSize", "50");
        properties.Add("Packet Size", "8192");
        properties.Add("ServerType", "0");
        return string.Join(";", properties.Select(p => $"{p.Key}={p.Value}"));
    }

    /// <summary>
    /// Executes the SQL script in the FirebirdSql container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath =
            string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "/usr/local/firebird/bin/isql", "-user", _configuration.Username, "-pass", _configuration.Password, "-i", scriptFilePath, $"localhost:{_configuration.Database}" }, ct)
            .ConfigureAwait(false);
    }
}
