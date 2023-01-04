namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <summary>
  /// A resource configuration.
  /// </summary>
  [PublicAPI]
  public interface IResourceConfiguration
  {
    /// <summary>
    /// Gets the test session id.
    /// </summary>
    Guid SessionId { get; }

    /// <summary>
    /// Gets the Docker endpoint authentication configuration.
    /// </summary>
    IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <summary>
    /// Gets a list of labels.
    /// </summary>
    IReadOnlyDictionary<string, string> Labels { get; }
  }
}
