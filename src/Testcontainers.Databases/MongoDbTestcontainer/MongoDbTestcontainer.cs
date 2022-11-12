namespace DotNet.Testcontainers.Containers
{
  using System.Collections.Generic;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <inheritdoc cref="TestcontainerDatabase" />
  [PublicAPI]
  public sealed class MongoDbTestcontainer : TestcontainerDatabase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbTestcontainer" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    internal MongoDbTestcontainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <inheritdoc />
    public override string ConnectionString
      => string.IsNullOrEmpty(this.Username) && string.IsNullOrEmpty(this.Password)
        ? $"mongodb://{this.Hostname}:{this.Port}"
        : $"mongodb://{this.Username}:{this.Password}@{this.Hostname}:{this.Port}";

    /// <summary>
    /// Executes a MongoDB Shell script in the database container.
    /// </summary>
    /// <param name="scriptContent">The content of the MongoDB Shell script to be executed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the script has been executed.</returns>
    public override async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
      var tempScriptFile = this.GetTempScriptFile();

      var command = new List<string> { "mongosh", "--eval", $"load('{tempScriptFile}')" };

      if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
      {
        command.Add("--username");
        command.Add(this.Username);
        command.Add("--password");
        command.Add(this.Password);
      }

      await this.CopyFileAsync(tempScriptFile, Encoding.Default.GetBytes(scriptContent), 493, 0, 0, ct)
        .ConfigureAwait(false);

      return await this.ExecAsync(command, ct)
        .ConfigureAwait(false);
    }
  }
}
