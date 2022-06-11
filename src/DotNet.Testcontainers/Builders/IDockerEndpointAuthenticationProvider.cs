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
    /// Is true when the authentication provider contains any credentials, otherwise false.
    /// </summary>
    /// <returns>True when the authentication provider contains any credentials, otherwise false.</returns>
    [PublicAPI]
    bool IsApplicable();

    /// <summary>
    /// Gets the Docker authentication configuration.
    /// </summary>
    /// <returns>The Docker authentication configuration or null if no configuration matches the hostname.</returns>
    [PublicAPI]
    [CanBeNull]
    IDockerEndpointAuthenticationConfiguration GetAuthConfig();
  }
}
