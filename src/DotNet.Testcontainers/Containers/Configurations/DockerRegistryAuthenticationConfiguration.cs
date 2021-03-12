namespace DotNet.Testcontainers.Containers.Configurations
{
  using System;

  /// <inheritdoc cref="IDockerRegistryAuthenticationConfiguration" />
  /// <remarks>In the future, we will replace this class. Instead, we will use the local Docker credentials.</remarks>
  internal sealed class DockerRegistryAuthenticationConfiguration : IDockerRegistryAuthenticationConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DockerRegistryAuthenticationConfiguration" /> class.
    /// </summary>
    /// <param name="registryEndpoint">The Docker registry endpoint.</param>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    public DockerRegistryAuthenticationConfiguration(
      Uri registryEndpoint = null,
      string username = null,
      string password = null)
    {
      this.RegistryEndpoint = registryEndpoint;
      this.Username = username;
      this.Password = password;
    }

    /// <summary>
    /// Gets the default Docker client auth configuration.
    /// </summary>
    public static IDockerRegistryAuthenticationConfiguration Default { get; }
      = new DockerRegistryAuthenticationConfiguration();

    /// <inheritdoc />
    public Uri RegistryEndpoint { get; }

    /// <inheritdoc />
    public string Username { get; }

    /// <inheritdoc />
    public string Password { get; }
  }
}
