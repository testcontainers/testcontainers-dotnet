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
        return string.Join(
            ";",
            properties.Select(property => string.Join("=", property.Key, property.Value))
        );
    }

    /// <summary>
    /// Executes the SQL script in the MariaDb container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(
        string scriptContent,
        CancellationToken ct = default
    )
    {
        var scriptFilePath = string.Join(
            "/",
            string.Empty,
            "tmp",
            Guid.NewGuid().ToString("D"),
            Path.GetRandomFileName()
        );

        await CopyAsync(
                Encoding.Default.GetBytes(scriptContent),
                scriptFilePath,
                Unix.FileMode644,
                ct
            )
            .ConfigureAwait(false);

        return await ExecAsync(
                new[] { "mariadb", _configuration.Database, $"--execute=source {scriptFilePath};" },
                ct
            )
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Write an unobfuscated MariaDb configuration file that configures the client
    /// login path. This prevents warnings in the <see cref="ExecScriptAsync" />
    /// result about using a password on the command line.
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
        return CopyAsync(
            Encoding.Default.GetBytes(config.ToString()),
            "/etc/mysql/my.cnf",
            UnixFileModes.UserRead | UnixFileModes.UserWrite,
            ct
        );
    }
}
