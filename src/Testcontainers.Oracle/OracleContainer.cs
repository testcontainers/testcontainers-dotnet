namespace Testcontainers.Oracle;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OracleContainer : DockerContainer, IDatabaseContainer
{
    private readonly OracleConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public OracleContainer(OracleConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Oracle connection string.
    /// </summary>
    /// <returns>The Oracle connection string.</returns>
    public string GetConnectionString()
    {
        const string dataSource = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4};";
        return string.Format(dataSource, Hostname, GetMappedPublicPort(OracleBuilder.OraclePort), _configuration.Database, _configuration.Username, _configuration.Password);
    }

    /// <summary>
    /// Executes the SQL script in the Oracle container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "/bin/sh", "-c", $"exit | sqlplus -LOGON -SILENT {_configuration.Username}/{_configuration.Password}@localhost:1521/{_configuration.Database} @{scriptFilePath}" }, ct)
            .ConfigureAwait(false);
    }
}