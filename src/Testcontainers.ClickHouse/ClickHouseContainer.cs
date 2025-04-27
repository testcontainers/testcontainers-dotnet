namespace Testcontainers.ClickHouse;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ClickHouseContainer : DockerContainer, IDatabaseContainer
{
    private readonly ClickHouseConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ClickHouseContainer(ClickHouseConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the ClickHouse connection string.
    /// </summary>
    /// <returns>The ClickHouse connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Host", Hostname);
        properties.Add("Port", GetMappedPublicPort(ClickHouseBuilder.HttpPort).ToString());
        properties.Add("Database", _configuration.Database);
        properties.Add("Username", _configuration.Username);
        properties.Add("Password", _configuration.Password);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Executes the SQL script in the ClickHouse container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "clickhouse-client", "--database", _configuration.Database, "--queries-file", scriptFilePath }, ct)
            .ConfigureAwait(false);
    }
}