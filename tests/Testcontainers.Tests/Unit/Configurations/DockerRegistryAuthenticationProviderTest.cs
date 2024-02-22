namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Runtime.InteropServices;
  using System.Text.Json;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Images;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class DockerRegistryAuthenticationProviderTest
  {
    private const string DockerRegistry = "https://index.docker.io/v1/";

    [Theory]
    [InlineData("baz/foo/bar:1.0.0", null)]
    [InlineData("baz/foo/bar", null)]
    [InlineData("baz/foo/bar:latest", null)]
    [InlineData("foo/bar:1.0.0", null)]
    [InlineData("foo/bar", null)]
    [InlineData("foo/bar:latest", null)]
    [InlineData("bar:1.0.0", null)]
    [InlineData("bar:latest", null)]
    [InlineData("myregistry.azurecr.io/baz/foo/bar:1.0.0", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/baz/foo/bar", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/baz/foo/bar:latest", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/bar:1.0.0", "myregistry.azurecr.io")]
    [InlineData("fedora/httpd:version1.0.test", null)]
    [InlineData("fedora/httpd:version1.0", null)]
    [InlineData("myregistryhost:5000/fedora/httpd:version1.0", "myregistryhost:5000")]
    [InlineData("myregistryhost:5000/httpd:version1.0", "myregistryhost:5000")]
    [InlineData("baz/.foo/bar:1.0.0", null)]
    [InlineData("baz/:foo/bar:1.0.0", null)]
    [InlineData("myregistry.azurecr.io/baz.foo/bar:1.0.0", "myregistry.azurecr.io")]
    [InlineData("myregistry.azurecr.io/baz:foo/bar:1.0.0", "myregistry.azurecr.io")]
    public void GetHostnameFromDockerImage(string dockerImageName, string hostname)
    {
      IImage image = new DockerImage(dockerImageName);
      Assert.Equal(hostname, image.GetHostname());
    }

    [Theory]
    [InlineData("", "docker", "stable")]
    [InlineData("fedora", "httpd", "1.0")]
    [InlineData("foo/bar", "baz", "1.0.0")]
    public void GetHostnameFromHubImageNamePrefix(string repository, string name, string tag)
    {
      const string hubImageNamePrefix = "myregistry.azurecr.io";
      IImage image = new DockerImage(repository, name, tag, hubImageNamePrefix);
      Assert.Equal(hubImageNamePrefix, image.GetHostname());
    }

    [Fact]
    public void ShouldGetDefaultDockerRegistryAuthenticationConfiguration()
    {
      var authenticationProvider = new DockerRegistryAuthenticationProvider("/tmp/docker.config", NullLogger.Instance);
      Assert.Equal(default(DockerRegistryAuthenticationConfiguration), authenticationProvider.GetAuthConfig("index.docker.io"));
    }

    public sealed class Base64ProviderTest
    {
      private readonly WarnLogger _warnLogger = new WarnLogger();

      [Theory]
      [InlineData("{\"auths\":{\"ghcr.io\":{}}}")]
      [InlineData("{\"auths\":{\"://ghcr.io\":{}}}")]
      public void ResolvePartialDockerRegistry(string jsonDocument)
      {
        // Given
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new Base64Provider(jsonElement, NullLogger.Instance);

        // Then
        Assert.False(authenticationProvider.IsApplicable("ghcr"));
        Assert.True(authenticationProvider.IsApplicable("ghcr.io"));
      }

      [Theory]
      [InlineData("{}", false, null)]
      [InlineData("{\"auths\":null}", false, null)]
      [InlineData("{\"auths\":{}}", false, null)]
      [InlineData("{\"auths\":{\"ghcr.io\":{}}}", false, null)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{}}}", true, null)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":null}}}", true, "The \"auth\" property value for https://index.docker.io/v1/ not found")]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":\"\"}}}", true, "The \"auth\" property value for https://index.docker.io/v1/ not found")]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":{}}}}", true, "The \"auth\" property value kind for https://index.docker.io/v1/ is invalid: Object")]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":\"Not_Base64_encoded\"}}}", true, "The \"auth\" property value for https://index.docker.io/v1/ is not a valid Base64 string")]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":\"dXNlcm5hbWU=\"}}}", true, "The \"auth\" property value for https://index.docker.io/v1/ should contain one colon separating the username and the password (basic authentication)")]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"identitytoken\":null}}}", true, null)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"identitytoken\":\"\"}}}", true, null)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"identitytoken\":{}}}}", true, null)]
      public void ShouldGetNull(string jsonDocument, bool isApplicable, string logMessage)
      {
        // Given
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new Base64Provider(jsonElement, _warnLogger);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.Equal(isApplicable, authenticationProvider.IsApplicable(DockerRegistry));
        Assert.Null(authConfig);

        if (string.IsNullOrEmpty(logMessage))
        {
          Assert.Empty(_warnLogger.LogMessages);
        }
        else
        {
          Assert.Single(_warnLogger.LogMessages, item => logMessage.Equals(item.Item2));
        }
      }

      [Theory]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"auth\":\"dXNlcm5hbWU6cGFzc3dvcmQ=\"}}}", "username", "password", null)]
      [InlineData("{\"auths\":{\"" + DockerRegistry + "\":{\"identitytoken\":\"identitytoken\"}}}", null, null, "identitytoken")]
      public void ShouldGetAuthConfig(string jsonDocument, string expectedUsername, string expectedPassword, string expectedIdentityToken)
      {
        // Given
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new Base64Provider(jsonElement, NullLogger.Instance);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.True(authenticationProvider.IsApplicable(DockerRegistry));
        Assert.NotNull(authConfig);
        Assert.Equal(DockerRegistry, authConfig.RegistryEndpoint);
        Assert.Equal(expectedUsername, authConfig.Username);
        Assert.Equal(expectedPassword, authConfig.Password);
        Assert.Equal(expectedIdentityToken, authConfig.IdentityToken);
      }
    }

    public sealed class CredsStoreProviderTest : SetEnvVarPath
    {
      [Theory]
      [InlineData("{}", false)]
      [InlineData("{\"credsStore\":null}", false)]
      [InlineData("{\"credsStore\":\"\"}", false)]
      [InlineData("{\"credsStore\":\"script.sh\"}", true)]
      public void ShouldGetNull(string jsonDocument, bool isApplicable)
      {
        // Given
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new CredsStoreProvider(jsonElement, NullLogger.Instance);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.Equal(isApplicable, authenticationProvider.IsApplicable(DockerRegistry));
        Assert.Null(authConfig);
      }

      [Fact]
      public void ShouldGetAuthConfig()
      {
        // Given
        var credsStoreScriptName = Path.ChangeExtension("desktop", RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh");
        var jsonDocument = "{\"credsStore\":\"" + credsStoreScriptName + "\"}";
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new CredsStoreProvider(jsonElement, NullLogger.Instance);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.True(authenticationProvider.IsApplicable(DockerRegistry));
        Assert.NotNull(authConfig);
        Assert.Equal(DockerRegistry, authConfig.RegistryEndpoint);
        Assert.Equal("username", authConfig.Username);
        Assert.Equal("password", authConfig.Password);
      }
    }

    public sealed class CredsHelperProviderTest : SetEnvVarPath
    {
      [Theory]
      [InlineData("{}", false)]
      [InlineData("{\"credHelpers\":null}", false)]
      [InlineData("{\"credHelpers\":{}}", false)]
      [InlineData("{\"credHelpers\":{\"ghcr.io\":{}}}", false)]
      [InlineData("{\"credHelpers\":{\"" + DockerRegistry + "\":{}}}", true)]
      [InlineData("{\"credHelpers\":{\"" + DockerRegistry + "\":null}}", true)]
      [InlineData("{\"credHelpers\":{\"" + DockerRegistry + "\":\"\"}}", true)]
      [InlineData("{\"credHelpers\":{\"" + DockerRegistry + "\":\"script.sh\"}}", true)]
      public void ShouldGetNull(string jsonDocument, bool isApplicable)
      {
        // Given
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new CredsHelperProvider(jsonElement, NullLogger.Instance);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.Equal(isApplicable, authenticationProvider.IsApplicable(DockerRegistry));
        Assert.Null(authConfig);
      }

      [Theory]
      [InlineData("password", "username", "password", null)]
      [InlineData("token", null, null, "identitytoken")]
      public void ShouldGetAuthConfig(string credHelperName, string expectedUsername, string expectedPassword, string expectedIdentityToken)
      {
        // Given
        var credHelpersScriptName = Path.ChangeExtension(credHelperName, RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh");
        var jsonDocument = "{\"credHelpers\":{\"" + DockerRegistry + "\":\"" + credHelpersScriptName + "\"}}";
        var jsonElement = JsonDocument.Parse(jsonDocument).RootElement;

        // When
        var authenticationProvider = new CredsHelperProvider(jsonElement, NullLogger.Instance);
        var authConfig = authenticationProvider.GetAuthConfig(DockerRegistry);

        // Then
        Assert.True(authenticationProvider.IsApplicable(DockerRegistry));
        Assert.NotNull(authConfig);
        Assert.Equal(DockerRegistry, authConfig.RegistryEndpoint);
        Assert.Equal(expectedUsername, authConfig.Username);
        Assert.Equal(expectedPassword, authConfig.Password);
        Assert.Equal(expectedIdentityToken, authConfig.IdentityToken);
      }
    }

    public abstract class SetEnvVarPath
    {
      static SetEnvVarPath()
      {
        Environment.SetEnvironmentVariable("PATH", string.Join(Path.PathSeparator, (Environment.GetEnvironmentVariable("PATH") ?? string.Empty)
          .Split(Path.PathSeparator)
          .Prepend(Path.Combine(Environment.CurrentDirectory, "Assets", "credHelpers"))
          .Prepend(Path.Combine(Environment.CurrentDirectory, "Assets", "credsStore"))
          .Distinct()));
      }
    }

    private sealed class Disposable : IDisposable
    {
      static Disposable()
      {
      }

      private Disposable()
      {
      }

      public static IDisposable Empty { get; }
        = new Disposable();

      public void Dispose()
      {
      }
    }

    private sealed class WarnLogger : ILogger
    {
      private readonly IList<Tuple<LogLevel, string>> _logMessages = new List<Tuple<LogLevel, string>>();

      public IEnumerable<Tuple<LogLevel, string>> LogMessages => _logMessages;

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
        if (IsEnabled(logLevel))
        {
          _logMessages.Add(Tuple.Create(logLevel, formatter.Invoke(state, exception)));
        }
      }

      public bool IsEnabled(LogLevel logLevel)
      {
        return LogLevel.Warning.Equals(logLevel);
      }

      public IDisposable BeginScope<TState>(TState state)
      {
        return Disposable.Empty;
      }
    }
  }
}
