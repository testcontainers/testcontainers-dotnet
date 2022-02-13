namespace DotNet.Testcontainers.Tests
{
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Xunit;

  public sealed class Initialization : IHostedService
  {
    public Initialization(ILogger logger, FactAttribute skipOnLinuxEngine)
    {
      TestcontainersSettings.ResourceReaperEnabled = skipOnLinuxEngine.Skip != null;
      TestcontainersSettings.Logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      return Task.CompletedTask;
    }
  }
}
