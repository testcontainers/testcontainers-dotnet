namespace DotNet.Testcontainers.Configurations
{
  using JetBrains.Annotations;

  /// <summary>
  /// Provides host specific operation system information to start Docker container.
  /// </summary>
  public interface IOperatingSystem
  {
    /// <summary>
    /// Gets the Docker endpoint authentication configuration.
    /// </summary>
    [PublicAPI]
    IDockerEndpointAuthenticationConfiguration DockerEndpointAuthConfig { get; }

    /// <summary>
    /// Modifies a string-path that it matches the operating system directory separator.
    /// </summary>
    /// <param name="path">Path to normalize.</param>
    /// <returns>Normalized path.</returns>
    [PublicAPI]
    string NormalizePath(string path);
  }
}
