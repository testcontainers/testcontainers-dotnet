namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.InteropServices;
  using Docker.DotNet;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Diagnostics;

  internal sealed class TestcontainersClient : ITestcontainersClient
  {
    private static readonly Uri Endpoint = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");

    private static readonly DockerClient Docker = new DockerClientConfiguration(Endpoint).CreateClient();

    private static readonly Lazy<ITestcontainersClient> Testcontainers = new Lazy<ITestcontainersClient>(() => new TestcontainersClient());

    private TestcontainersClient()
    {
    }

    public static ITestcontainersClient Instance
    {
      get
      {
        return Testcontainers.Value;
      }
    }

    public bool HasImage(string image)
    {
      if (string.IsNullOrWhiteSpace(image))
      {
        return false;
      }

      return Docker.Images.ListImagesAsync(new ImagesListParameters
      {
        MatchName = image,
      }).Result.Any();
    }

    public bool HasContainer(string containerId)
    {
      if (string.IsNullOrWhiteSpace(containerId))
      {
        return false;
      }

      return Docker.Containers.ListContainersAsync(new ContainersListParameters
      {
        All = true,
        Filters = new Dictionary<string, IDictionary<string, bool>>
        {
          {
            "id", new Dictionary<string, bool>
            {
              { containerId, true },
            }
          },
        },
      }).Result.Any();
    }

    public void Pull(string image)
    {
      Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance).Wait();
    }

    public void Start(string containerId)
    {
      Docker.Containers.StartContainerAsync(containerId, new ContainerStartParameters { }).Wait();
    }

    public void Stop(string containerId)
    {
      Docker.Containers.StopContainerAsync(containerId, new ContainerStopParameters { WaitBeforeKillSeconds = 30 }).Wait();
    }

    public string Run(string image, HostConfig hostConfig)
    {
      return Docker.Containers.CreateContainerAsync(new CreateContainerParameters
      {
        Image = image,
        HostConfig = hostConfig,
      }).Result.ID;
    }
  }
}
