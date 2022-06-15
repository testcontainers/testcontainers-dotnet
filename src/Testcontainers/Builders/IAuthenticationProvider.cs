namespace DotNet.Testcontainers.Builders
{
  using JetBrains.Annotations;

  /// <summary>
  /// A Docker authentication provider.
  /// </summary>
  /// <typeparam name="TAuthenticationConfiguration">Type of the authentication configuration.</typeparam>
  [PublicAPI]
  internal interface IAuthenticationProvider<out TAuthenticationConfiguration>
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
    /// <param name="hostname">The Docker hostname.</param>
    /// <returns>The Docker authentication configuration or null if no configuration matches the hostname.</returns>
    [PublicAPI]
    [CanBeNull]
    TAuthenticationConfiguration GetAuthConfig(string hostname);
  }
}
