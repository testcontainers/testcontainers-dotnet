namespace DotNet.Testcontainers.Configurations
{
  using System;
  using JetBrains.Annotations;

  /// <summary>
  /// A authentication configuration to authenticate against private Docker registries.
  /// </summary>
  public interface IDockerRegistryAuthenticationConfiguration
  {
    /// <summary>
    /// Gets the Docker registry endpoint.
    /// </summary>
    [CanBeNull]
    Uri RegistryEndpoint { get; }

    /// <summary>
    /// Gets the username.
    /// </summary>
    [CanBeNull]
    string Username { get; }

    /// <summary>
    /// Gets the password.
    /// </summary>
    [CanBeNull]
    string Password { get; }
  }
}
