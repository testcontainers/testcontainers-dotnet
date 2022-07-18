namespace DotNet.Testcontainers.Configurations
{
  using System;
  using Docker.DotNet;
  using JetBrains.Annotations;

  /// <summary>
  /// An authentication configuration to authenticate against private Docker clients.
  /// </summary>
  public interface IDockerEndpointAuthenticationConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// Gets the Docker API credentials.
    /// </summary>
    [CanBeNull]
    Credentials Credentials { get; }

    /// <summary>
    /// Gets the Docker client configuration.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <returns>The Docker client configuration.</returns>
    [NotNull]
    DockerClientConfiguration GetDockerClientConfiguration(Guid sessionId = default);
  }
}
