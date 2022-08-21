namespace DotNet.Testcontainers.Containers
{
  using System.Globalization;
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

    public async Task<ExecResult> ExecMongoScriptAsync(string script, CancellationToken ct = default)
    {
      var tempScriptFile = this.GetTempScriptFile();

      await this.CopyFileAsync(tempScriptFile, Encoding.Default.GetBytes(script), 493, 0, 0, ct)
                      .ConfigureAwait(false);

      return await this.ExecAsync(new[] { "mongo", "db", tempScriptFile }, ct);
    }

    public override async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
      var tempScriptFile = this.GetTempScriptFile();

      await this.CopyFileAsync(tempScriptFile, Encoding.Default.GetBytes(scriptContent), 493, 0, 0, ct)
                      .ConfigureAwait(false);

      return await this.ExecAsync(new[] { "mongosh", "--port", this.ContainerPort.ToString(CultureInfo.InvariantCulture), "--eval", $"load('{tempScriptFile}')" }, ct)
                        .ConfigureAwait(false);
    }
  }
}
