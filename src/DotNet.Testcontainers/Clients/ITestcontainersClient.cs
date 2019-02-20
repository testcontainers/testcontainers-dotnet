namespace DotNet.Testcontainers.Clients
{
  using Docker.DotNet.Models;

  public interface ITestcontainersClient
  {
    bool HasImage(string imageName);

    bool HasContainer(string containerId);

    string GetImageName(string containerId);

    string GetContainerName(string containerId);

    void Pull(string imageName);

    void Start(string containerId);

    void Stop(string containerId);

    string Run(string containerName, string image, HostConfig hostConfig);
  }
}
