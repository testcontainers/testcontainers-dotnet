namespace DotNet.Testcontainers.Configurations
{
  using System;

  /// <summary>
  /// Unix operating system.
  /// </summary>
  internal readonly struct Unix : IOperatingSystem
  {
    private static readonly Uri Endpoint = new Uri("unix:/var/run/docker.sock");

    /// <inheritdoc />
    public Uri DockerApiEndpoint
      => Endpoint;

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('\\', '/');
    }
  }
}
