namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// An authentication configuration to authenticate against private Docker registries.
  /// </summary>
  public interface IDockerRegistryAuthenticationConfiguration
  {
    /// <summary>
    /// Gets the Docker registry endpoint.
    /// </summary>
    [CanBeNull]
    string RegistryEndpoint { get; }

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

    /// <summary>
    /// Gets the identity token.
    /// </summary>
    [CanBeNull]
    string IdentityToken { get; }
  }
}
