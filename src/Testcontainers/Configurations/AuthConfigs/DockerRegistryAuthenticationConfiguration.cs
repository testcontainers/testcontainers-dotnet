namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.Text.Json;

  /// <inheritdoc cref="IDockerRegistryAuthenticationConfiguration" />
  internal readonly struct DockerRegistryAuthenticationConfiguration : IDockerRegistryAuthenticationConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationConfiguration" /> struct.
    /// </summary>
    /// <param name="registryEndpoint">The Docker registry endpoint.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <param name="identityToken">The identity token.</param>
    public DockerRegistryAuthenticationConfiguration(
      string registryEndpoint = null,
      string username = null,
      string password = null,
      string identityToken = null)
    {
      RegistryEndpoint = registryEndpoint;
      Username = username;
      Password = password;
      IdentityToken = identityToken;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationConfiguration" /> struct.
    /// </summary>
    /// <param name="registryEndpoint">The Docker registry endpoint.</param>
    /// <param name="credential">The CredHelpers or CredsStore JSON response.</param>
    public DockerRegistryAuthenticationConfiguration(
      string registryEndpoint,
      JsonElement credential)
    {
      var username = credential.TryGetProperty(nameof(Username), out var usernameProperty) ? usernameProperty.GetString() : null;

      var password = credential.TryGetProperty("Secret", out var passwordProperty) ? passwordProperty.GetString() : null;

      if ("<token>".Equals(username, StringComparison.OrdinalIgnoreCase))
      {
        RegistryEndpoint = registryEndpoint;
        Username = null;
        Password = null;
        IdentityToken = password;
      }
      else
      {
        RegistryEndpoint = registryEndpoint;
        Username = username;
        Password = password;
        IdentityToken = null;
      }
    }

    /// <inheritdoc />
    public string RegistryEndpoint { get; }

    /// <inheritdoc />
    public string Username { get; }

    /// <inheritdoc />
    public string Password { get; }

    /// <inheritdoc />
    public string IdentityToken { get; }
  }
}
