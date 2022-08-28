namespace DotNet.Testcontainers.Containers
{
  using System;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class MsSqlTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal MsSqlTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    public override string ConnectionString
      => $"Server={this.Hostname},{this.Port};Database={this.Database};User Id={this.Username};Password={this.Password};";

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

      return await this.ExecAsync(new[] { "/opt/mssql-tools/bin/sqlcmd", "-b", "-r", "1", "-S", $"{this.Hostname},{this.ContainerPort}", "-U", this.Username, "-P", this.Password, "-i", tempScriptFile }, ct)
        .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public override async Task StartAsync(CancellationToken ct = default)
    {
      await base.StartAsync(ct)
        .ConfigureAwait(false);

      // MSSQL contains the master database by default. It's not necessary to create it.
      if (MsSqlTestcontainerConfiguration.MasterDatabase.Equals(this.Database, StringComparison.OrdinalIgnoreCase))
      {
        return;
      }

      // Replace this with proper SQL args, soon as we moved to dedicated modules.
      var createDatabaseScript = $@"
        IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{this.Database}')
          BEGIN
            CREATE DATABASE [{this.Database}];
          END;
      ";

      await this.ExecScriptAsync(createDatabaseScript, ct)
        .ConfigureAwait(false);
    }
  }
}
