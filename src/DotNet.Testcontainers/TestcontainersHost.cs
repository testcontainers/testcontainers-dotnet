namespace DotNet.Testcontainers
{
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Serilog;
  using ILogger = Microsoft.Extensions.Logging.ILogger;

  internal static class TestcontainersHost
  {
    private static readonly IHost host;

    static TestcontainersHost()
    {
      var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

      var logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

      host = new HostBuilder()
        .ConfigureLogging(config => config.AddSerilog(logger, true))
        .Build();
    }

    internal static ILogger GetLogger(string categoryName)
    {
      return host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(categoryName);
    }

    internal static ILogger<T> GetLogger<T>()
    {
      return host.Services.GetRequiredService<ILogger<T>>();
    }
  }
}
