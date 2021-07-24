namespace DotNet.Testcontainers.Tests
{
  using Microsoft.Extensions.Configuration;
  using Serilog;
  using Serilog.Extensions.Logging;

  internal sealed class CustomSerilogLoggerFactory : SerilogLoggerFactory
  {
    public CustomSerilogLoggerFactory(IConfiguration configuration)
      : base(
        new LoggerConfiguration()
          .ReadFrom
          .Configuration(configuration)
          .CreateLogger(),
        true)
    {
    }
  }
}
