namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// A Docker resource configuration.
  /// </summary>
  public interface IDockerResourceConfiguration
  {
    /// <summary>
    /// Gets the session id.
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
