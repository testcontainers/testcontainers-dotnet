namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Concurrent;
  using Docker.DotNet;

  internal abstract class DockerApiClient
  {
    private static readonly ConcurrentDictionary<Uri, IDockerClient> Clients = new ConcurrentDictionary<Uri, IDockerClient>();

    protected DockerApiClient(Uri endpoint, Credentials credentials = null) : this(
      Clients.GetOrAdd(endpoint, _ => new DockerClientConfiguration(endpoint, credentials).CreateClient()))
    {
    }

    protected DockerApiClient(IDockerClient client)
    {
      this.Docker = client;
    }

    protected IDockerClient Docker { get; }
  }
}
