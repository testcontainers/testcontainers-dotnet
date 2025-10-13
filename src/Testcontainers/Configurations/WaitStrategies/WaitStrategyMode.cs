namespace DotNet.Testcontainers.Configurations
{
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// Defines the execution mode for wait strategies.
  /// </summary>
  [PublicAPI]
  public enum WaitStrategyMode
  {
    /// <summary>
    /// Indicates that the container is expected to be in the <c>Running</c> state.
    /// </summary>
    /// <remarks>
    /// When this mode is used, the library verifies that the container is running. If
    /// the container is not running, it collects the container's <c>stdout</c> and
    /// <c>stderr</c> logs and throws a <see cref="ContainerNotRunningException"/> exception.
    /// </remarks>
    Running,

    /// <summary>
    /// Executes the wait strategy a single time without requiring the container to be running.
    /// </summary>
    /// <remarks>
    /// This mode does not check the container's running state and does not retry.
    /// </remarks>
    OneShot,
  }
}
