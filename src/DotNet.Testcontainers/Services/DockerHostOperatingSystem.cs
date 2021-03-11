namespace DotNet.Testcontainers.Services
{
  using System;

  internal readonly struct DockerHostOperatingSystem : IOperatingSystem
  {
    internal const string DockerHostEnvName = "DOCKER_HOST";

    private readonly IOperatingSystem os;

    public DockerHostOperatingSystem(IOperatingSystem os) : this()
    {
      this.os = os;
      this.DockerApiEndpoint = new Uri(Environment.GetEnvironmentVariable(DockerHostEnvName)!);
    }

    public Uri DockerApiEndpoint { get; }

    public string NormalizePath(string path) => this.os.NormalizePath(path);
  }
}
