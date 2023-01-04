namespace DotNet.Testcontainers.Containers
{
  using System.Threading;
  using System.Threading.Tasks;
  using JetBrains.Annotations;

  /// <summary>
  /// Define the contract for database modules that has the script execution capability.
  /// </summary>
  public interface IDatabaseScript
  {
    /// <summary>
    /// Executes a script in the running Testcontainer.
    /// </summary>
    /// <param name="scriptContent">The content of the script to be executed.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the script has been executed.</returns>
    [PublicAPI]
    Task<ExecResult> ExecScriptAsync(string scriptContent, CancellationToken ct = default);
  }
}
