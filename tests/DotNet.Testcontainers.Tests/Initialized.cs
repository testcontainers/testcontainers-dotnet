namespace DotNet.Testcontainers.Tests
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Xunit;

  [CollectionDefinition(nameof(Testcontainers))]
  public sealed class Initialized : ICollectionFixture<Initialized>, IAsyncLifetime, IDisposable
  {
    private readonly IHost host = Host
      .CreateDefaultBuilder()
      .ConfigureServices(serviceCollection =>
      {
        serviceCollection.AddSingleton<ILoggerFactory, CustomSerilogLoggerFactory>();
        serviceCollection.AddSingleton<ILogger>(serviceProvider => serviceProvider.GetRequiredService<ILogger<Initialized>>());
        serviceCollection.AddSingleton<FactAttribute, SkipOnLinuxEngineAttribute>();
        serviceCollection.AddHostedService<Initialization>();
      })
      .Build();

    public Task InitializeAsync()
    {
      return this.host.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.host.StopAsync();
    }

    public void Dispose()
    {
      this.host.Dispose();
    }
  }
}
