namespace Testcontainers.Cassandra;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class CassandraContainer : DockerContainer, IDatabaseContainer
{
    /// <inheritdoc cref="DockerContainer" />
    public CassandraContainer(CassandraConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Cassandra connection string.
    /// </summary>
    /// <returns>The Cassandra connection string.</returns>
    public string GetConnectionString()
    {
        var properties = new Dictionary<string, string>();
        properties.Add("Contact Points", Hostname);
        properties.Add("Port", GetMappedPublicPort(CassandraBuilder.CqlPort).ToString());
        return string.Join(";", properties.Select(property => string.Join("=", property.Key, property.Value)));
    }

    /// <summary>
    /// Executes the CQL script in the Cassandra container.
    /// </summary>
    /// <param name="scriptContent">The content of the CQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the CQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
        var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

        await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
            .ConfigureAwait(false);

        return await ExecAsync(new[] { "cqlsh", "--file", scriptFilePath }, ct)
            .ConfigureAwait(false);
    }
}