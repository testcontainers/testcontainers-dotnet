namespace DotNet.Testcontainers.Containers
{
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class OracleTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="OracleTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal OracleTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    public override string ConnectionString
      => $"Data Source={this.Hostname}:{this.Port};User id={this.Username};Password={this.Password};";

    /// <summary>
    /// Executes a SQL script in the database container.
    /// </summary>
    /// <param name="scriptContent">The content of the SQL script to be executed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the script has been executed.</returns>
    public override async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
      var tempScriptFile = this.GetTempScriptFile();

      await this.CopyFileAsync(tempScriptFile, Encoding.Default.GetBytes(scriptContent), 493, 0, 0, ct)
        .ConfigureAwait(false);

      return await this.ExecAsync(new[] { "/bin/sh", "-c", $"exit | $ORACLE_HOME/bin/sqlplus -S {this.Username}/{this.Password}@{this.Hostname} @{tempScriptFile}" }, ct)
        .ConfigureAwait(false);
    }
  }
}
