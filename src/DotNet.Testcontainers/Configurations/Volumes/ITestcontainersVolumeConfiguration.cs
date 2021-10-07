namespace DotNet.Testcontainers.Configurations.Volumes
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker volume configuration.
  /// </summary>
  internal interface ITestcontainersVolumeConfiguration
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

    /// <summary>
    /// Gets a list of labels.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> Labels { get; }
  }
}
