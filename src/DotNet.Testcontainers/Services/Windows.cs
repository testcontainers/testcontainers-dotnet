namespace DotNet.Testcontainers.Services
{
  using System;

  /// <summary>
  /// Windows operating system.
  /// </summary>
  internal readonly struct Windows : IOperatingSystem
  {
    private static readonly Uri endpoint = new Uri("npipe://./pipe/docker_engine");

    /// <inheritdoc />
    public Uri DockerApiEndpoint => endpoint;

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('/', '\\');
    }
  }
}
