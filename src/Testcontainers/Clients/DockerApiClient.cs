namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Concurrent;
  using Docker.DotNet;
  using DotNet.Testcontainers.Configurations;

  internal abstract class DockerApiClient
  {
    private static readonly ConcurrentDictionary<Uri, IDockerClient> Clients = new ConcurrentDictionary<Uri, IDockerClient>();

    protected DockerApiClient(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      _ = sessionId;
      this.Docker = Clients.GetOrAdd(dockerEndpointAuthConfig.Endpoint, _ =>
      {
        using (var dockerClientConfiguration = dockerEndpointAuthConfig.GetDockerClientConfiguration())
        {
          return dockerClientConfiguration.CreateClient();
        }
      });
    }

    protected IDockerClient Docker { get; }
  }
}
