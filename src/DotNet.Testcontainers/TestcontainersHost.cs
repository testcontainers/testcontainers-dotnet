namespace DotNet.Testcontainers
{
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Serilog;
  using ILogger = Microsoft.Extensions.Logging.ILogger;

  internal static class TestcontainersHost
  {
    private static readonly IHost host = InitHost();

    internal static ILogger GetLogger(string categoryName)
    {
      return host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(categoryName);
    }

    internal static ILogger<T> GetLogger<T>()
    {
      return host.Services.GetRequiredService<ILogger<T>>();
    }

    private static IHost InitHost()
    {
      return new HostBuilder()
        .ConfigureLogging(config => config.AddSerilog(TestcontainersLoggerConfiguration.Production.CreateLogger(), true))
        .Build();
    }
  }
}
