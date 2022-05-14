namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using Docker.DotNet;

  /// <summary>
  /// A Docker resource configuration.
  /// </summary>
  public interface IDockerResourceConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    Uri Endpoint { get; }

    /// <summary>
    /// Gets a list of labels.
    /// </summary>
    IReadOnlyDictionary<string, string> Labels { get; }

    /// <summary>
    /// Gets the Docker API credentials.
    /// </summary>
    Credentials Credentials { get; }
  }
}
