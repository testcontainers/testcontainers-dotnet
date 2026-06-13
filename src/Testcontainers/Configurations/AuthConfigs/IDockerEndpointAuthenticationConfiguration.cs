namespace DotNet.Testcontainers.Configurations
{
  using System;
  using Docker.DotNet;
  using Docker.DotNet.Handler.Abstractions;
  using JetBrains.Annotations;

  /// <summary>
  /// An authentication configuration to authenticate against private Docker clients.
  /// </summary>
  [PublicAPI]
  public interface IDockerEndpointAuthenticationConfiguration
  {
    /// <summary>
    /// Gets the Docker API version.
    /// </summary>
    [CanBeNull]
    Version Version { get; }

    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// Gets the Docker API authentication provider.
    /// </summary>
    [CanBeNull]
    IAuthProvider AuthProvider { get; }

    /// <summary>
    /// Gets the Docker client builder.
    /// </summary>
    /// <param name="sessionId">The session id.</param>
    /// <returns>The Docker client builder.</returns>
    [NotNull]
    DockerClientBuilder GetDockerClientBuilder(Guid sessionId = default);
  }
}
