namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker endpoint authentication provider.
  /// </summary>
  [PublicAPI]
  internal interface IDockerEndpointAuthenticationProvider
  {
    /// <summary>
    /// Is true when the authentication provider contains Docker endpoint credentials, otherwise false.
    /// </summary>
    /// <returns>True when the authentication provider contains Docker endpoint credentials, otherwise false.</returns>
    [PublicAPI]
    bool IsApplicable();

    /// <summary>
    /// Is true when a connection to the Docker endpoint can be established, otherwise false.
    /// </summary>
    /// <returns>True when a connection to the Docker endpoint can be established, otherwise false.</returns>
    [PublicAPI]
    bool IsAvailable();

    /// <summary>
    /// Gets the Docker endpoint authentication configuration.
    /// </summary>
    /// <returns>The Docker endpoint authentication configuration or null if no configuration matches the hostname.</returns>
    [PublicAPI]
    [CanBeNull]
    IDockerEndpointAuthenticationConfiguration GetAuthConfig();
  }
}
