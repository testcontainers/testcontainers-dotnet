namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.Json;
  using DotNet.Testcontainers.Builders;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public class Base64ProviderTest
  {
    [Fact]
    public void ShouldDecodeAuth()
    {
      // Given
      // lang=json
      const string config = """
      {
        "auths": {
          "https://index.docker.io/v1/": {
            "auth": "dXNlcjpwYXNzd29yZA=="
          }
        }
      }
      """;
      var provider = new Base64Provider(JsonDocument.Parse(config), NullLogger.Instance);

      // When
      var authConfig = provider.GetAuthConfig("https://index.docker.io/v1/");

      // Then
      Assert.NotNull(authConfig);
      Assert.Equal("https://index.docker.io/v1/", authConfig.RegistryEndpoint);
      Assert.Equal("user", authConfig.Username);
      Assert.Equal("password", authConfig.Password);
      Assert.Null(authConfig.IdentityToken);
    }

    [Theory]
    [InlineData("null", "The \"auth\" value for https://index.docker.io/v1/ is missing")]
    [InlineData("\"\"", "The \"auth\" value for https://index.docker.io/v1/ is missing")]
    [InlineData("{}", "The \"auth\" value for https://index.docker.io/v1/ is invalid (Object instead of String)")]
    [InlineData("\"not base 64\"", "The \"auth\" value for https://index.docker.io/v1/ is not a valid base64 string")]
    [InlineData("\"ww==\"", "The \"auth\" value for https://index.docker.io/v1/, once base64 decoded, should contain one and only one colon separating the user name and password")] // invalid UTF8 (single 0xC3 byte)
    [InlineData("\"Xw==\"", "The \"auth\" value for https://index.docker.io/v1/, once base64 decoded, should contain one and only one colon separating the user name and password")] // valid base 64 but contains no colon separator
    public void ShouldNotDecodeAuth(string auth, string expectedMessage)
    {
      // Given
      // lang=json
      var config = $$"""
      {
        "auths": {
          "https://index.docker.io/v1/": {
            "auth": {{auth}}
          }
        }
      }
      """;
      var recorder = new LogRecorder();
      var provider = new Base64Provider(JsonDocument.Parse(config), recorder);

      // When
      var authConfig = provider.GetAuthConfig("https://index.docker.io/v1/");

      // Then
      Assert.Null(authConfig);
      Assert.Equal(expectedMessage, Assert.Single(recorder.Logs.Where(e => e.Level == LogLevel.Warning).Select(e => e.Text)));
    }

    private class LogRecorder : ILogger
    {
      private readonly List<(LogLevel Level, string Text)> _logs = new List<(LogLevel Level, string Text)>();

      public IEnumerable<(LogLevel Level, string Text)> Logs => _logs;

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
        _logs.Add((logLevel, formatter(state, exception)));
      }

      public bool IsEnabled(LogLevel logLevel) => true;

      public IDisposable BeginScope<TState>(TState state) => null;
    }
  }
}
