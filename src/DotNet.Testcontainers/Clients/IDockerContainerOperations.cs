namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;

  internal interface IDockerContainerOperations : IHasListOperations<ContainerListResponse>
  {
    Task<long> GetExitCode(string id, CancellationToken ct = default);

    Task StartAsync(string id, CancellationToken ct = default);

    Task StopAsync(string id, CancellationToken ct = default);

    Task RemoveAsync(string id, CancellationToken ct = default);

    Task ExtractArchiveToContainerAsync(string id, string path, Stream tarStream, CancellationToken ct = default);

    Task AttachAsync(string id, IOutputConsumer outputConsumer, CancellationToken ct = default);

    Task<ExecResult> ExecAsync(string id, IList<string> command, CancellationToken ct = default);

    Task<string> RunAsync(ITestcontainersConfiguration configuration, CancellationToken ct = default);
  }
}
