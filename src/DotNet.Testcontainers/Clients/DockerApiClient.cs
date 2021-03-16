namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Concurrent;
  using Docker.DotNet;
  using DotNet.Testcontainers.Containers.Configurations;
  using DockerClientConfiguration = Docker.DotNet.DockerClientConfiguration;

  internal abstract class DockerApiClient
  {
    private static readonly ConcurrentDictionary<Uri, IDockerClient> Clients = new ConcurrentDictionary<Uri, IDockerClient>();

    // TODO: Add certificate credentials here.
    protected DockerApiClient(IDockerClientConfiguration clientConfig)
      : this(Clients.GetOrAdd(clientConfig.Endpoint, _ => new DockerClientConfiguration(clientConfig.Endpoint, clientConfig.Credentials).CreateClient()))
    {
    }

    protected DockerApiClient(IDockerClient client)
    {
      this.Docker = client;
    }

    protected IDockerClient Docker { get; }
  }
}
