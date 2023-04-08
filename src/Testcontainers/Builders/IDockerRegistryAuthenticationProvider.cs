namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker registry authentication provider.
  /// </summary>
  [PublicAPI]
  internal interface IDockerRegistryAuthenticationProvider
  {
    /// <summary>
    /// Is true when the authentication provider contains any Docker registry credentials, otherwise false.
    /// </summary>
    /// <param name="hostname">The Docker hostname.</param>
    /// <returns>True when the authentication provider contains any Docker registry credentials, otherwise false.</returns>
    [PublicAPI]
    bool IsApplicable(string hostname);

    /// <summary>
    /// Gets the Docker registry authentication configuration.
    /// </summary>
    /// <param name="hostname">The Docker hostname.</param>
    /// <returns>The Docker registry authentication configuration or null if no configuration matches the hostname.</returns>
    [PublicAPI]
    [CanBeNull]
    IDockerRegistryAuthenticationConfiguration GetAuthConfig(string hostname);
  }
}
