namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Concurrent;
  using Docker.DotNet;
  using DotNet.Testcontainers.Configurations;

  internal abstract class DockerApiClient
  {
    private static readonly ConcurrentDictionary<Uri, Lazy<IDockerClient>> Clients = new ConcurrentDictionary<Uri, Lazy<IDockerClient>>();

    protected DockerApiClient(Guid sessionId, IDockerEndpointAuthenticationConfiguration dockerEndpointAuthConfig)
    {
      var lazyDockerClient = Clients.GetOrAdd(dockerEndpointAuthConfig.Endpoint, _ =>
        new Lazy<IDockerClient>(() =>
        {
          using (var dockerClientConfiguration = dockerEndpointAuthConfig.GetDockerClientConfiguration(sessionId))
          {
            return dockerClientConfiguration.CreateClient();
          }
        }));

      Docker = lazyDockerClient.Value;
    }

    protected IDockerClient Docker { get; }
  }
}
