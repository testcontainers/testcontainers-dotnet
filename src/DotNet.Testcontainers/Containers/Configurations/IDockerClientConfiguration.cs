namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;
  using Docker.DotNet;
  using JetBrains.Annotations;

  /// <summary>
  /// An authentication configuration to authenticate against Docker hosts (TLS).
  /// </summary>
  public interface IDockerClientConfiguration
  {
    /// <summary>
    /// Gets the Docker API endpoint.
    /// </summary>
    [NotNull]
    Uri Endpoint { get; }

    /// <summary>
    /// True if the auth configuration is applicable, otherwise false.
    /// </summary>
    bool IsApplicable { get; }

    /// <summary>
    /// Credentials used to authenticate with the Docker daemon e.g. client certificates
    /// </summary>
    [CanBeNull]
    Credentials Credentials { get; }
  }
}
