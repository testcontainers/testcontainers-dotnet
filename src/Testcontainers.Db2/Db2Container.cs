namespace Testcontainers.Db2;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class Db2Container : DockerContainer, IDatabaseContainer
{
    private readonly Db2Configuration _configuration;

    public Db2Container(Db2Configuration configuration) : base(configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Gets the Db2 connection string.
    /// </summary>
    /// <returns>The Db2 connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Server", Hostname + ":" + GetMappedPublicPort(Db2Builder.Db2Port));
        properties.Add("Database", _configuration.Database);
        properties.Add("UID", _configuration.Username);
        properties.Add("PWD", _configuration.Password);
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Executes the SQL script in the Db2 container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the SQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        const string db2ShellCommandFormat = "su - {1} -c \"db2 connect to {0} && db2 -tvf '{2}'\"";

        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        var db2ShellCommand = string.Format(db2ShellCommandFormat, _configuration.Database, _configuration.Username, scriptFilePath);

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new [] { "/bin/sh", "-c", db2ShellCommand}, ct)
            .ConfigureAwait(false);
    }
}