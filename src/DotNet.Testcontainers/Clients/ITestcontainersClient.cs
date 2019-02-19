namespace DotNet.Testcontainers.Clients
{
  using System.Threading.Tasks;
  using Docker.DotNet.Models;

  public interface ITestcontainersClient
  {
    bool HasImage(string image);

    bool HasContainer(string containerId);

    Task Pull(string image);

    string Run(string image, HostConfig hostConfig);

    Task Start(string containerId);

    Task Stop(string containerId);
  }
}
