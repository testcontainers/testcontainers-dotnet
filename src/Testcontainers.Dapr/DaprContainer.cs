namespace Testcontainers.Dapr;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class DaprContainer : DockerContainer
{
    private readonly DaprConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="Daprontainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public DaprContainer(DaprConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the DaprContainer connection string.
    /// </summary>
    /// <returns>The PostgreSql connection string.</returns>
    // public string GetConnectionString()
    // {
    //     var properties = new Dictionary<string, string>();
    //     properties.Add("Host", Hostname);
    //     properties.Add("Port", GetMappedPublicPort(DaprBuilder.PostgreSqlPort).ToString());
    //     properties.Add("Database", _configuration.Database);
    //     properties.Add("Username", _configuration.Username);
    //     properties.Add("Password", _configuration.Password);
    //     return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    // }

    public int DaprHttpPort
    {
        get { return GetMappedPublicPort(_configuration.DaprHttpPort); }
    }

    public int DaprGrpcPort
    {
        get { return GetMappedPublicPort(_configuration.DaprGrpcPort); }
    }

    /// <summary>
    /// Executes the SQL script in the PostgreSql container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    // public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    // {
    //     var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

    //     await CopyFileAsync(scriptFilePath, Encoding.Default.GetBytes(scriptContent), 493, 0, 0, ct)
    //         .ConfigureAwait(false);

    //     return await ExecAsync(new[] { "psql", "--username", _configuration.Username, "--dbname", _configuration.Database, "--file", scriptFilePath }, ct)
    //         .ConfigureAwait(false);
    // }
}