namespace DotNet.Testcontainers.Containers
{
  using JetBrains.Annotations;

  /// <summary>
  /// Docker container states.
  /// </summary>
  [PublicAPI]
  public enum TestcontainersState
  {
    /// <summary>
    /// Docker container was not created.
    /// </summary>
    [PublicAPI]
    Undefined,

    /// <summary>
    /// Docker container is created.
    /// </summary>
    [PublicAPI]
    Created,

    /// <summary>
    /// Docker container is restarting.
    /// </summary>
    [PublicAPI]
    Restarting,

    /// <summary>
    /// Docker container is running.
    /// </summary>
    [PublicAPI]
    Running,

    /// <summary>
    /// Docker container is paused.
    /// </summary>
    [PublicAPI]
    Paused,

    /// <summary>
    /// Docker container is exited.
    /// </summary>
    [PublicAPI]
    Exited,

    /// <summary>
    /// Docker container is dead.
    /// </summary>
    [PublicAPI]
    Dead,
  }
}
