namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker registry authentication provider.
  /// </summary>
  [PublicAPI]
  internal interface IAuthenticationProvider
  {
    /// <summary>
    /// Is true when the authentication provider contains any credentials, otherwise false.
    /// </summary>
    /// <returns>True when the authentication provider contains any credentials, otherwise false.</returns>
    [PublicAPI]
    bool IsApplicable();

    /// <summary>
    /// Gets the Docker registry authentication configuration.
    /// </summary>
    /// <param name="hostname">The Docker registry hostname.</param>
    /// <returns>The Docker registry authentication configuration or null if no configuration matches the hostname.</returns>
    [PublicAPI]
    [CanBeNull]
    IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname);
  }
}
