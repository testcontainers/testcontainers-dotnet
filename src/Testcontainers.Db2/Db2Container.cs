namespace Testcontainers.Db2;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class Db2Container : DockerContainer, IDatabaseContainer
{
    private static string Db2CommandPath = "/opt/ibm/db2/*/bin/db2";

    private readonly Db2Configuration _configuration;

    private const char ConnectionStringDelimiter = ';';

    public Db2Container(Db2Configuration configuration) : base(configuration)
    {
        _configuration = configuration;
    }

    public string GetConnectionString() => new StringBuilder()
        .Append("Server=").Append(Hostname).Append(':').Append(GetMappedPublicPort(Db2Builder.Db2Port).ToString())
        .Append(ConnectionStringDelimiter)
        .Append("Database=").Append(_configuration.Database).Append(ConnectionStringDelimiter)
        .Append("UID=").Append(_configuration.Username).Append(ConnectionStringDelimiter)
        .Append("PWD=").Append(_configuration.Password).Append(ConnectionStringDelimiter)
        .ToString();

    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken cancellationToken = default)
    {
        string[] command =
        [
            "su", "db2inst1", "-c", new StringBuilder()
                .Append(Db2CommandPath).Append(" connect to ").Append(_configuration.Database)
                .Append(" user ").Append(_configuration.Username).Append(" using ").Append(_configuration.Password)
                .Append("; ")
                .Append(Db2CommandPath).Append(' ').Append(scriptContent)
                .ToString()
        ];

        return await ExecAsync(command).ConfigureAwait(false);
    }
}