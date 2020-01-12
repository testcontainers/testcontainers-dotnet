namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet;

  internal class DockerApiClient
  {
    protected DockerApiClient(Uri endpoint) : this(new DockerClientConfiguration(endpoint).CreateClient())
    {
      this.Docker = new DockerClientConfiguration(endpoint).CreateClient();
    }

    protected DockerApiClient(IDockerClient client)
    {
      this.Docker = client;
    }

    protected IDockerClient Docker { get; }
  }
}
