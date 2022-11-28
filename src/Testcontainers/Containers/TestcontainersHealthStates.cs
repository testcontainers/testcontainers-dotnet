namespace DotNet.Testcontainers.Containers
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// Docker container health state.
  /// </summary>
  [PublicAPI]
  [Flags]
  public enum TestcontainersHealthStates
  {
    /// <summary>
    /// Docker container is healthy and ready for use.
    /// </summary>
    [PublicAPI]
    Healthy = 0x1,

    /// <summary>
    /// Docker container is not working correctly.
    /// </summary>
    [PublicAPI]
    Unhealthy = 0x2,
  }
}
