namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Docker container health status.
  /// </summary>
  [PublicAPI]
  [Flags]
  public enum TestcontainersHealthStatus
  {
    /// <summary>
    /// Docker container has not been created.
    /// </summary>
    [PublicAPI]
    Undefined = 0x1,

    /// <summary>
    /// Docker container has no health check assigned.
    /// </summary>
    [PublicAPI]
    None = 0x2,

    /// <summary>
    /// Docker container is starting.
    /// </summary>
    [PublicAPI]
    Starting = 0x4,

    /// <summary>
    /// Docker container is healthy.
    /// </summary>
    [PublicAPI]
    Healthy = 0x8,

    /// <summary>
    /// Docker container is unhealthy.
    /// </summary>
    [PublicAPI]
    Unhealthy = 0x10,
  }
}
