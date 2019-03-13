namespace DotNet.Testcontainers.Clients
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Diagnostics;

  internal interface ITestcontainersClient
  {
    Task StartAsync(string id);

    Task StopAsync(string id);

    Task RemoveAsync(string id);

    Task AttachAsync(string id, IOutputConsumer outputConsumer);

    Task ExecAsync(string id, params string[] command);

    Task<string> RunAsync(TestcontainersConfiguration config);
  }
}
