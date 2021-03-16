namespace DotNet.Testcontainers.Services
{
  using System;

  /// <summary>
  /// Windows operating system.
  /// </summary>
  internal readonly struct Windows : IOperatingSystem
  {
#pragma warning disable S1075

    private static readonly Uri endpoint = new Uri("npipe://./pipe/docker_engine");

#pragma warning restore S1075

    /// <inheritdoc />
    public Uri DockerApiEndpoint => endpoint;

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('/', '\\');
    }
  }
}
