namespace DotNet.Testcontainers.Configurations
{
  using System.Collections.Generic;

  /// <summary>
  /// A Docker resource configuration.
  /// </summary>
  public interface IDockerResourceConfiguration
  {
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
