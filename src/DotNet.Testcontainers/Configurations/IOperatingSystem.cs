namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <summary>
  /// Provides host specific operation system information to start Docker container.
  /// </summary>
  public interface IOperatingSystem
  {
    /// <summary>
    /// Gets the default host Docker daemon endpoint address.
    /// </summary>
    Uri DockerApiEndpoint { get; }

    /// <summary>
    /// Modifies a string-path that it matches the operating system directory separator.
    /// </summary>
    /// <param name="path">Path to normalize.</param>
    /// <returns>Normalized path.</returns>
    string NormalizePath(string path);
  }
}
