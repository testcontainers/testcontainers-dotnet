namespace DotNet.Testcontainers.Containers.Modules
{
  /// <summary>
  /// Docker container states.
  /// </summary>
  public enum TestcontainersState
  {
    /// <summary>
    /// Docker container was not created.
    /// </summary>
    Undefined,

    /// <summary>
    /// Docker container is created.
    /// </summary>
    Created,

    /// <summary>
    /// Docker container is restarting.
    /// </summary>
    Restarting,

    /// <summary>
    /// Docker container is running.
    /// </summary>
    Running,

    /// <summary>
    /// Docker container is paused.
    /// </summary>
    Paused,

    /// <summary>
    /// Docker container is exited.
    /// </summary>
    Exited,

    /// <summary>
    /// Docker container is dead.
    /// </summary>
    Dead,
  }
}
