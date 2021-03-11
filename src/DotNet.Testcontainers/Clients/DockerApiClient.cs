namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Concurrent;
  using Docker.DotNet;
  using DotNet.Testcontainers.Containers.Configurations;

  internal abstract class DockerApiClient
  {
    private static readonly ConcurrentDictionary<Uri, IDockerClient> Clients = new ConcurrentDictionary<Uri, IDockerClient>();

    // TODO: Add certificate credentials here.
    protected DockerApiClient(IDockerClientAuthenticationConfiguration clientAuthConfig)
      : this(Clients.GetOrAdd(clientAuthConfig.Endpoint, _ => new DockerClientConfiguration(clientAuthConfig.Endpoint).CreateClient()))
    {
    }

    protected DockerApiClient(IDockerClient client)
    {
      this.Docker = client;
    }

    protected IDockerClient Docker { get; }
  }
}
