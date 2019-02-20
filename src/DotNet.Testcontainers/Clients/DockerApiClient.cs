namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Runtime.InteropServices;
  using Docker.DotNet;

  internal class DockerApiClient
  {
    protected static readonly Uri Endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");

    protected static readonly DockerClient Docker = new DockerClientConfiguration(Endpoint).CreateClient();

    protected DockerApiClient()
    {
    }
  }
}
