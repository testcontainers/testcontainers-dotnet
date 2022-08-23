namespace DotNet.Testcontainers.Containers
{
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// This class represents an extended configured Testcontainer for databases.
  /// </summary>
  [PublicAPI]
  public abstract class TestcontainerDatabase : HostedServiceContainer, IDatabaseScript
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainerDatabase" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    protected TestcontainerDatabase(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the database connection string.
    /// </summary>
    [PublicAPI]
    public abstract string ConnectionString { get; }

    /// <summary>
    /// Gets or sets the database.
    /// </summary>
    [PublicAPI]
    public virtual string Database { get; set; }

    /// <summary>
    /// Creates a path to a temporary script file.
    /// </summary>
    /// <returns>A path to a temporary script file.</returns>
    [PublicAPI]
    public virtual string GetTempScriptFile()
    {
      return Path.Combine("/tmp/", Path.GetRandomFileName());
    }

    /// <summary>
    /// Executes a bash script in the database container.
    /// </summary>
    /// <param name="scriptContent">The content of the bash script to be executed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the script has been executed.</returns>
    public virtual async Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default)
    {
      var tempScriptFile = this.GetTempScriptFile();

      await this.CopyFileAsync(tempScriptFile, Encoding.Default.GetBytes(scriptContent), 493 /* 755 */, 0, 0, ct)
        .ConfigureAwait(false);

      return await this.ExecAsync(new[] { "/bin/sh", "-c", tempScriptFile }, ct)
        .ConfigureAwait(false);
    }
  }
}
