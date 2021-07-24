namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Concurrent;
  using Docker.DotNet;

  internal abstract class DockerApiClient
  {
    private static readonly ConcurrentDictionary<Uri, IDockerClient> Clients = new ConcurrentDictionary<Uri, IDockerClient>();

    protected DockerApiClient(Uri endpoint)
    {
      this.Docker = Clients.GetOrAdd(endpoint, _ => new DockerClientConfiguration(endpoint).CreateClient());
    }

    protected IDockerClient Docker { get; }
  }
}
