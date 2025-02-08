namespace Testcontainers.Cassandra
{
  /// <inheritdoc cref="DockerContainer" />
  [PublicAPI]
  public class CassandraContainer : DockerContainer, IDatabaseContainer
  {
    private readonly CassandraConfiguration _configuration;

    /// <inheritdoc cref="DockerContainer" />
    public CassandraContainer(CassandraConfiguration configuration) : base(configuration)
    {
      _configuration = configuration;
    }

    public IPEndPoint GetEndPoint()
    {
      return new IPEndPoint(IPAddress.Loopback, GetMappedPublicPort(CassandraBuilder.CqlPort));
    }

    public string GetConnectionString()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Executes the SQL script in the Cassandra container.
    /// </summary>
    /// <param name="scriptContent">The content of the CQL script to execute.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the CQL script has been executed.</returns>
    public async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
      var scriptFilePath = string.Join("/", string.Empty, "tmp", Guid.NewGuid().ToString("D"), Path.GetRandomFileName());

      await CopyAsync(Encoding.Default.GetBytes(scriptContent), scriptFilePath, Unix.FileMode644, ct)
        .ConfigureAwait(false);

      return await ExecAsync(new[] { "cqlsh", "-f", scriptFilePath }, ct)
        .ConfigureAwait(false);
    }

  }
}
