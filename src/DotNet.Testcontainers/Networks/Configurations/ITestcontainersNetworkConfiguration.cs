namespace DotNet.Testcontainers.Networks.Configurations
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker network configuration.
  /// </summary>
  internal interface ITestcontainersNetworkConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// Gets the driver.
    /// </summary>
    NetworkDriver Driver { get; }

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
