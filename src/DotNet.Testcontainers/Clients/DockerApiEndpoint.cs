namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Runtime.InteropServices;

  internal static class DockerApiEndpoint
  {
#pragma warning disable S1075

    private static readonly Uri Unix = new Uri("unix:/var/run/docker.sock");

    private static readonly Uri Windows = new Uri("npipe://./pipe/docker_engine");

#pragma warning restore S1075

    public static readonly Uri Local = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Windows : Unix;
  }
}
