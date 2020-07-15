namespace DotNet.Testcontainers.Clients
{
  using System;
  using DotNet.Testcontainers.Services;

  internal static class DockerApiEndpoint
  {
    public static Uri Local => TestcontainersHostService.GetService<IOperatingSystem>().DockerApiEndpoint;
  }
}
