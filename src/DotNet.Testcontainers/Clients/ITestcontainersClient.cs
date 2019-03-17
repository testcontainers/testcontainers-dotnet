namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Diagnostics;

  internal interface ITestcontainersClient
  {
    Task StartAsync(string id, CancellationToken cancellationToken = default(CancellationToken));

    Task StopAsync(string id, CancellationToken cancellationToken = default(CancellationToken));

    Task RemoveAsync(string id, CancellationToken cancellationToken = default(CancellationToken));

    Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken cancellationToken = default(CancellationToken));

    Task ExecAsync(string id, params string[] command);

    Task<string> RunAsync(TestcontainersConfiguration config);
  }
}
