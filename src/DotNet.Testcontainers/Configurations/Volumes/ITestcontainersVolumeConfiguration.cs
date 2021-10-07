namespace DotNet.Testcontainers.Configurations.Volumes
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker volume configuration.
  /// </summary>
  public interface ITestcontainersVolumeConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [NotNull]
    string Name { get; }
  }
}
