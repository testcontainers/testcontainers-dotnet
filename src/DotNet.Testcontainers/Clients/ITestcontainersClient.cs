namespace DotNet.Testcontainers.Clients
{
  using DotNet.Testcontainers.Core.Models;

  internal interface ITestcontainersClient
  {
    void Start(string id);

    void Stop(string id);

    void Remove(string id);

    string Run(TestcontainersConfiguration configuration);
  }
}
