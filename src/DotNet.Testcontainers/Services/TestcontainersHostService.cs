namespace DotNet.Testcontainers.Services
{
  using System.IO;
  using System.Runtime.InteropServices;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Serilog;
  using ILogger = Microsoft.Extensions.Logging.ILogger;

  /// <summary>
  /// Global service register.
  /// </summary>
  internal static class TestcontainersHostService
  {
    private const string SerilogSectionName = "Serilog";

    private static readonly IHost Host = InitHost();

    /// <summary>
    /// Gets logger of category name.
    /// </summary>
    /// <param name="categoryName">Name of the logger.</param>
    /// <returns>Returns a configured logger.</returns>
    public static ILogger GetLogger(string categoryName)
    {
      return Host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(categoryName);
    }

    /// <summary>
    /// Gets logger of type.
    /// </summary>
    /// <typeparam name="TCategoryName">Type of logger.</typeparam>
    /// <returns>Returns a configured logger.</returns>
    public static ILogger<TCategoryName> GetLogger<TCategoryName>()
    {
      return Host.Services.GetRequiredService<ILogger<TCategoryName>>();
    }

    /// <summary>
    /// Get service of type <typeparamref name="TServiceName" />.
    /// </summary>
    /// <typeparam name="TServiceName">Type of service.</typeparam>
    /// <returns>Returns a service object of type <typeparamref name="TServiceName" />.</returns>
    public static TServiceName GetService<TServiceName>()
    {
      return Host.Services.GetRequiredService<TServiceName>();
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
        .ConfigureServices((hostContext, config) =>
        {
          var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? (IOperatingSystem) new Windows() : new Unix();
          config.AddSingleton(os);
        })
        .Build();
    }
  }
}
