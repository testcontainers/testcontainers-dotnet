namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Docker container states.
  /// </summary>
  [PublicAPI]
  [Flags]
  public enum TestcontainersStates
  {
    /// <summary>
    /// Docker container has not been created.
    /// </summary>
    [PublicAPI]
    Undefined = 0x1,

    /// <summary>
    /// Docker container is created.
    /// </summary>
    [PublicAPI]
    Created = 0x2,

    /// <summary>
    /// Docker container is restarting.
    /// </summary>
    [PublicAPI]
    Restarting = 0x4,

    /// <summary>
    /// Docker container is running.
    /// </summary>
    [PublicAPI]
    Running = 0x8,

    /// <summary>
    /// Docker container is paused.
    /// </summary>
    [PublicAPI]
    Paused = 0x10,

    /// <summary>
    /// Docker container is exited.
    /// </summary>
    [PublicAPI]
    Exited = 0x20,

    /// <summary>
    /// Docker container is dead.
    /// </summary>
    [PublicAPI]
    Dead = 0x40,
  }
}
