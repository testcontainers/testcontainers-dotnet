namespace DotNet.Testcontainers.Clients
{
  using System;
  using DotNet.Testcontainers.Services;

  internal static class DockerApiEndpoint
  {
    public static Uri Default => TestcontainersHostService.GetService<IOperatingSystem>().DockerApiEndpoint;
  }
}
