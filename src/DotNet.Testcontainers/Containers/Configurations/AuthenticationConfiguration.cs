namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;

  /// <inheritdoc cref="IAuthenticationConfiguration" />
  /// <remarks>In the future, we will replace this class. Instead, we will use the local Docker credentials.</remarks>
  internal readonly struct AuthenticationConfiguration : IAuthenticationConfiguration
  {
    /// <summary>
    /// Creates a <see cref="AuthenticationConfiguration" />.
    /// </summary>
    /// <param name="registryEndpoint">The Docker registry endpoint.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    public AuthenticationConfiguration(
      Uri registryEndpoint = null,
      string username = null,
      string password = null)
    {
      this.RegistryEndpoint = registryEndpoint;
      this.Username = username;
      this.Password = password;
    }

    /// <inheritdoc />
    public Uri RegistryEndpoint { get; }

    /// <inheritdoc />
    public string Username { get; }

    /// <inheritdoc />
    public string Password { get; }
  }
}
