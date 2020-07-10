namespace DotNet.Testcontainers.Services
{
  using System.IO;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Serilog;
  using ILogger = Microsoft.Extensions.Logging.ILogger;

  internal static class TestcontainersHostService
  {
    private const string SerilogSectionName = "Serilog";

    private static readonly IHost Host = InitHost();

    public static ILogger GetLogger(string categoryName)
    {
      return Host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(categoryName);
    }

    public static ILogger<T> GetLogger<T>()
    {
      return Host.Services.GetRequiredService<ILogger<T>>();
    }

    private static IHost InitHost()
    {
      return new HostBuilder()
        .ConfigureAppConfiguration((hostContext, config) =>
        {
          config.SetBasePath(Directory.GetCurrentDirectory());
          config.AddJsonFile("appsettings.json", true, true);
          config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment}.json", true, true);
        })
        .ConfigureServices((hostContext, config) =>
        {
          var logger = new LoggerConfiguration().ReadFrom.Configuration(hostContext.Configuration, SerilogSectionName).CreateLogger();
          config.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger, true));
        })
        .Build();
    }
  }
}
