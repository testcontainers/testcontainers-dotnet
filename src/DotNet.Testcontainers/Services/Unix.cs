namespace DotNet.Testcontainers.Services
{
  using System;
  using static IOperatingSystem;

  /// <summary>
  /// Unix operating system.
  /// </summary>
  internal readonly struct Unix : IOperatingSystem
  {
    private const string DefaultDockerSocketPath = "unix:/var/run/docker.sock";

    private static readonly Uri endpoint = new Uri(Environment.GetEnvironmentVariable(DockerHostEnvName) ?? DefaultDockerSocketPath);

    /// <inheritdoc />
    public Uri DockerApiEndpoint => endpoint;

    /// <inheritdoc />
    public string NormalizePath(string path)
    {
      return path.Replace('\\', '/');
    }
  }
}
