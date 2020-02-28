namespace DotNet.Testcontainers.Internals
{
  using Microsoft.Extensions.Configuration;
  using Serilog;

  internal sealed class TestcontainersLoggerConfiguration : LoggerConfiguration
  {
    private TestcontainersLoggerConfiguration(string configuration) : this(
      new ConfigurationBuilder().AddJsonFile(configuration, true, true)
        .Build())
    {
    }

    private TestcontainersLoggerConfiguration(IConfiguration configuration)
    {
      this.ReadFrom.Configuration(configuration);
    }

    public static TestcontainersLoggerConfiguration Production =>
      new TestcontainersLoggerConfiguration("appsettings.json");

    public static TestcontainersLoggerConfiguration Development =>
      new TestcontainersLoggerConfiguration("appsettings.development.json");
  }
}
