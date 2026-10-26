namespace Testcontainers.MariaDb;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class MariaDbContainer : DockerContainer, IDatabaseContainer
{
    private readonly MariaDbConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public MariaDbContainer(MariaDbConfiguration configuration)
        : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the MariaDb connection string.
    /// </summary>
    /// <returns>The MariaDb connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Server", Hostname);
        properties.Add("Port", GetMappedPublicPort(MariaDbBuilder.MariaDbPort).ToString());
        properties.Add("Database", _configuration.Database);
        properties.Add("Uid", _configuration.Username);
        properties.Add("Pwd", _configuration.Password);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Executes the SQL script in the MariaDb container.
    /// </summary>
    /// <remarks>
    /// The script content is encoded using <see cref="Encoding.Default" />.
    /// </remarks>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        return ExecScriptAsync(scriptContent, Encoding.Default, ct);
    }

    /// <summary>
    /// Executes the SQL script in the MariaDb container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="encoding">The encoding used to write the script content to the container.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, Encoding encoding, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(encoding.GetBytes(scriptContent), scriptFilePath, fileMode: Unix.FileMode644, ct: ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "mariadb", _configuration.Database, $"--execute=source {scriptFilePath};" }, ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Writes an unobfuscated MariaDb configuration file that configures the client
    /// login path. This prevents warnings about using a password on the command
    /// line in the <see cref="ExecScriptAsync(string, CancellationToken)" /> result.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the configuration file has been executed.</returns>
    internal Task WriteConfigurationFileAsync(CancellationToken ct = default)
    {
        var config = new StringWriter();
        config.NewLine = "\n";
        config.WriteLine("[client]");
        config.WriteLine("protocol=TCP");
        config.WriteLine($"user={_configuration.Username}");
        config.WriteLine($"password={_configuration.Password}");
        return CopyAsync(Encoding.Default.GetBytes(config.ToString()), "/etc/mysql/my.cnf", fileMode: UnixFileModes.UserRead | UnixFileModes.UserWrite, ct: ct);
    }
}