namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Runtime.InteropServices;
  using Docker.DotNet;

  internal class DockerApiClient
  {
#pragma warning disable S1075

#if NET452 || NET462
    protected static readonly Uri Endpoint = new Uri("npipe://./pipe/docker_engine");
#else
    protected static readonly Uri Endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
#endif

#pragma warning restore S1075

    protected static readonly DockerClient Docker = new DockerClientConfiguration(Endpoint).CreateClient();

    protected DockerApiClient()
    {
    }
  }
}
