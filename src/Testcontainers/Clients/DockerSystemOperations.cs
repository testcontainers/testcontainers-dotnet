namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  internal sealed class DockerSystemOperations : DockerApiClient, IDockerSystemOperations
  {
    public DockerSystemOperations(IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig, ILogger logger)
      : base(dockerEndpointAuthConfig)
    {
      _ = logger;
    }

    public async Task<bool> GetIsWindowsEngineEnabled(CancellationToken ct = default)
    {
      return (await this.Docker.System.GetSystemInfoAsync(ct)
        .ConfigureAwait(false)).OperatingSystem.Contains("Windows");
    }
  }
}
