namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Defines a condition that is repeatedly evaluated until it becomes true.
  /// </summary>
  [PublicAPI]
  public interface IWaitUntil
  {
    /// <summary>
    /// Evaluates the condition asynchronously against the specified container.
    /// </summary>
    /// <param name="container">The container instance to check readiness against.</param>
    /// <returns>A task that returns <c>true</c> when the condition is satisfied; otherwise, <c>false</c>.</returns>
    Task<bool> UntilAsync(IContainer container);
  }
}
