namespace DotNet.Testcontainers.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Defines a condition that is repeatedly evaluated while it remains true.
  /// </summary>
  [PublicAPI]
  public interface IWaitWhile
  {
    /// <summary>
    /// Evaluates the condition asynchronously against the specified container.
    /// </summary>
    /// <param name="container">The container to check the condition for.</param>
    /// <returns>A task that returns <c>true</c> while the condition holds; otherwise, <c>false</c>.</returns>
    Task<bool> WhileAsync(IContainer container);
  }
}
