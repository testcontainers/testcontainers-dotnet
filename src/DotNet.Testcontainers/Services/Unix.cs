namespace DotNet.Testcontainers.Services
{
  using System;

  /// <summary>
  /// Unix operating system.
  /// </summary>
  internal readonly struct Unix : IOperatingSystem
  {
    private static readonly Uri endpoint = new Uri("unix:/var/run/docker.sock");

    /// <inheritdoc />
    public Uri DockerApiEndpoint => endpoint;

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('\\', '/');
    }
  }
}
