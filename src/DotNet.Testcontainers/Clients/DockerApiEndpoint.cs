namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet;
  using DotNet.Testcontainers.Services;

  internal static class DockerApiEndpoint
  {
    public static Uri Default => TestcontainersHostService.GetService<IOperatingSystem>().DockerApiEndpoint;

    public static Credentials DefaultCredentials => new AnonymousCredentials();
  }
}
