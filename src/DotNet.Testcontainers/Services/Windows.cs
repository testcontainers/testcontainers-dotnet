namespace DotNet.Testcontainers.Services
{
  using System;
  using static IOperatingSystem;

  /// <summary>
  /// Windows operating system.
  /// </summary>
  internal readonly struct Windows : IOperatingSystem
  {
#pragma warning disable S1075

    private const string DefaultDockerSocketPath = "npipe://./pipe/docker_engine";
    private static readonly Uri endpoint = new Uri(Environment.GetEnvironmentVariable(DockerHostEnvName) ?? DefaultDockerSocketPath);

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
