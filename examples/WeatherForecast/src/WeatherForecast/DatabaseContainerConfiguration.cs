using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace WeatherForecast;

public sealed class DatabaseContainerConfiguration : MsSqlTestcontainerConfiguration
{
  public DatabaseContainerConfiguration() : base("mcr.microsoft.com/azure-sql-edge:1.0.6")
  {
    Password = Guid.NewGuid().ToString("D");
    Database = Guid.NewGuid().ToString("D");
  }

  public override IWaitForContainerOS WaitStrategy { get; }
    = Wait.ForUnixContainer().AddCustomWaitStrategy(new AcceptsClientConnections());

  private sealed class AcceptsClientConnections : IWaitUntil
  {
    public async Task<bool> UntilAsync(IContainer container)
    {
      var (stdout, _) = await container.GetLogs()
        .ConfigureAwait(false);
      return stdout.Contains("SQL Server is now ready for client connections.", StringComparison.OrdinalIgnoreCase);
    }
  }
}
