namespace DotNet.Testcontainers.Clients
{
  using System;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Clients.MetaData;
  using DotNet.Testcontainers.Diagnostics;
  using static LanguageExt.Prelude;

  internal class TestcontainersClient : DockerApiClient, ITestcontainersClient
  {
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

    public bool HasImage(string imageName)
    {
      return Optional(imageName).Match(
        Some: name => notnull(MetaDataClientImages.Instance.ByName(name)),
        None: () => false);
    }

    public bool HasContainer(string containerId)
    {
      return Optional(containerId).Match(
        Some: id => notnull(MetaDataClientContainers.Instance.ByName(id)),
        None: () => false);
    }

    public string GetImageName(string imageName)
    {
      return Optional(imageName).Match(
        Some: name => MetaDataClientImages.Instance.ByName(name).ToString(),
        None: () => string.Empty);
    }

    public string GetContainerName(string containerId)
    {
      return Optional(containerId).Match(
        Some: id => MetaDataClientContainers.Instance.ById(id).ToString(),
        None: () => string.Empty);
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
      Docker.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { }).Wait();
    }

    public string Run(string name, string image, HostConfig hostConfig)
    {
      return Docker.Containers.CreateContainerAsync(new CreateContainerParameters
      {
        Name = name,
        Image = image,
        HostConfig = hostConfig,
      }).Result.ID;
    }
  }
}
