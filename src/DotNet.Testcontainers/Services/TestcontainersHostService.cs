namespace DotNet.Testcontainers.Services
{
  using DotNet.Testcontainers.Client;
  using DotNet.Testcontainers.Internals;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Serilog;
  using ILogger = Microsoft.Extensions.Logging.ILogger;

  public static class TestcontainersHostService
  {
    private static readonly IHost Host = InitHost();

    public static bool IsWindowsEngineEnabled => DockerApiClient.IsWindowsEngineEnabled;

    internal static ILogger GetLogger(string categoryName)
    {
      return Host.Services.GetRequiredService<ILoggerFactory>().CreateLogger(categoryName);
    }

    internal static ILogger<T> GetLogger<T>()
    {
      return Host.Services.GetRequiredService<ILogger<T>>();
    }

    private static IHost InitHost()
    {
      return new HostBuilder()
        .ConfigureLogging(config => config.AddSerilog(TestcontainersLoggerConfiguration.Production.CreateLogger(), true))
        .Build();
    }
  }
}
