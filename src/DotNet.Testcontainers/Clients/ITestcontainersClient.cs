namespace DotNet.Testcontainers.Clients
{
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Models;
  using DotNet.Testcontainers.Diagnostics;

  internal interface ITestcontainersClient
  {
    Task StartAsync(string id, CancellationToken cancellationToken = default);

    Task StopAsync(string id, CancellationToken cancellationToken = default);

    Task RemoveAsync(string id, CancellationToken cancellationToken = default);

    Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken cancellationToken = default);

    Task ExecAsync(string id, params string[] command);

    Task<string> BuildAsync(ImageFromDockerfileConfiguration config, CancellationToken cancellationToken = default);

    Task<string> RunAsync(TestcontainersConfiguration config, CancellationToken cancellationToken = default);
  }
}
