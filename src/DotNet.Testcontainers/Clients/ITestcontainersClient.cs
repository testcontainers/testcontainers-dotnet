namespace DotNet.Testcontainers.Clients
{
  using Docker.DotNet.Models;

  public interface ITestcontainersClient
  {
    bool HasImage(string image);

    bool HasContainer(string containerId);

    void Pull(string image);

    void Start(string containerId);

    void Stop(string containerId);

    string Run(string image, HostConfig hostConfig);
  }
}
