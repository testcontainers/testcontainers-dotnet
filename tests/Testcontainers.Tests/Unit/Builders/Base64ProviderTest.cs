using System.Text.Json;
using DotNet.Testcontainers.Builders;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DotNet.Testcontainers.Tests.Unit
{
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
    [InlineData("{}")]
    [InlineData("null")]
    [InlineData("\"not base 64\"")]
    [InlineData("\"ww==\"")] // invalid UTF8 (single 0xC3 byte)
    [InlineData("\"Xw==\"")] // valid base 64 but contains no colon separator
    public void ShouldNotDecodeAuth(string auth)
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
      var provider = new Base64Provider(JsonDocument.Parse(config), NullLogger.Instance);

      // When
      var authConfig = provider.GetAuthConfig("https://index.docker.io/v1/");

      // Then
      Assert.Null(authConfig);
    }
  }
}
