namespace DotNet.Testcontainers.Configurations
{
  using System;
  using Docker.DotNet;

  /// <summary>
  /// A authentication configuration to authenticate against private Docker clients.
  /// </summary>
  public interface IDockerEndpointAuthenticationConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    Uri Endpoint { get; }

    /// <summary>
    /// Gets the Docker client configuration.
    /// </summary>
    /// <returns>The Docker client configuration.</returns>
    DockerClientConfiguration GetDockerClientConfiguration();
  }
}
