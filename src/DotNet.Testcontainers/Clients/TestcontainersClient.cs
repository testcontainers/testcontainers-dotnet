namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Diagnostics;
  using static LanguageExt.Prelude;

  internal class TestcontainersClient : DockerApiClient, ITestcontainersClient
  {
    private static readonly Lazy<ITestcontainersClient> Testcontainers = new Lazy<ITestcontainersClient>(() => new TestcontainersClient());

    private TestcontainersClient()
    {
    }

    internal static ITestcontainersClient Instance
    {
      get
      {
        return Testcontainers.Value;
      }
    }

    public bool ExistImageById(string id)
    {
      return Optional(id).Match(
        Some: value => notnull(MetaDataClientImages.Instance.ById(value)),
        None: () => false);
    }

    public bool ExistImageByName(string name)
    {
      return Optional(name).Match(
        Some: value => notnull(MetaDataClientImages.Instance.ByName(value)),
        None: () => false);
    }

    public bool ExistContainerById(string id)
    {
      return Optional(id).Match(
        Some: value => notnull(MetaDataClientContainers.Instance.ById(value)),
        None: () => false);
    }

    public bool ExistContainerByName(string name)
    {
      return Optional(name).Match(
        Some: value => notnull(MetaDataClientContainers.Instance.ByName(value)),
        None: () => false);
    }

    public string FindImageNameById(string id)
    {
      return Optional(id).Match(
        Some: value => MetaDataClientImages.Instance.ById(value).ToString(),
        None: () => string.Empty);
    }

    public string FindImageNameByName(string name)
    {
      return Optional(name).Match(
        Some: value => MetaDataClientImages.Instance.ByName(value).ToString(),
        None: () => string.Empty);
    }

    public string FindContainerNameById(string id)
    {
      return Optional(id).Match(
        Some: value => MetaDataClientContainers.Instance.ById(value).ToString(),
        None: () => string.Empty);
    }

    public string FindContainerNameByName(string name)
    {
      return Optional(name).Match(
        Some: value => MetaDataClientContainers.Instance.ByName(value).ToString(),
        None: () => string.Empty);
    }

    public void Start(string id)
    {
      if (this.ExistContainerById(id))
      {
        Docker.Containers.StartContainerAsync(id, new ContainerStartParameters { }).Wait();
      }
    }

    public void Stop(string id)
    {
      if (this.ExistContainerById(id))
      {
        Docker.Containers.StopContainerAsync(id, new ContainerStopParameters { WaitBeforeKillSeconds = 15 }).Wait();
      }
    }

    public void Remove(string id)
    {
      if (this.ExistContainerById(id))
      {
        Docker.Containers.RemoveContainerAsync(id, new ContainerRemoveParameters { Force = true }).Wait();
      }
    }

    public string Run(DockerContainerConfig dockerContainerConfig, DockerHostConfig dockerHostConfig)
    {
      var image = dockerContainerConfig.Image;

      if (!this.ExistImageByName(image))
      {
        Docker.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = image }, null, DebugProgress.Instance).Wait();
      }

      var hostConfig = new HostConfig
      {
        PortBindings = dockerHostConfig.PortBindings,
        Mounts = dockerHostConfig.Mounts,
      };

      return Docker.Containers.CreateContainerAsync(new CreateContainerParameters
      {
        Image = image,
        Name = dockerContainerConfig.Name,
        Cmd = dockerContainerConfig.Command,
        HostConfig = hostConfig,
      }).Result.ID;
    }
  }
}
