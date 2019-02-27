namespace DotNet.Testcontainers.Clients
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Models;

  internal interface ITestcontainersClient
  {
    Task StartAsync(string id);

    Task StopAsync(string id);

    Task RemoveAsync(string id);

    Task<string> RunAsync(TestcontainersConfiguration configuration);
  }
}
