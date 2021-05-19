namespace DotNet.Testcontainers.Networks
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public interface IDockerNetwork
  {
    Task<IStartedNetwork> StartAsync();
  }

  public interface IStartedNetwork : IAsyncDisposable
  {
    string Name { get; }
    string Id { get; }
    Task StopAsync(CancellationToken ct);
  }
}
