namespace DotNet.Testcontainers.Clients
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.InteropServices;
  using System.Threading.Tasks;
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
      return Docker.Images.ListImagesAsync(new ImagesListParameters
      {
        MatchName = image,
      }).Result.Any();
    }

    public bool HasContainer(string containerId)
    {
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

    public Task Pull(string image)
    {
      return Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance);
    }

    public string Run(string image, HostConfig hostConfig)
    {
      if (!this.HasImage(image))
      {
        this.Pull(image);
      }

      return Docker.Containers.CreateContainerAsync(new CreateContainerParameters
      {
        Image = image,
        HostConfig = hostConfig,
      }).Result.ID;
    }

    public Task Start(string containerId)
    {
      return Docker.Containers.StartContainerAsync(containerId, new ContainerStartParameters { });
    }

    public Task Stop(string containerId)
    {
      return Docker.Containers.StopContainerAsync(containerId, new ContainerStopParameters { WaitBeforeKillSeconds = 30 });
    }
  }
}
