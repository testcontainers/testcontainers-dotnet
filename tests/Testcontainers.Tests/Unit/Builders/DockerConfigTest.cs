namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Runtime.CompilerServices;
  using System.Security.Cryptography;
  using System.Text;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public static class DockerConfigTests
  {
    public sealed class DockerContextConfigurationTests
    {
      [Fact]
      public void ReturnsActiveEndpointWhenDockerContextIsEmpty()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=" });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(DockerCli.GetCurrentEndpoint(), currentEndpoint);
      }

      [Fact]
      public void ReturnsDefaultEndpointWhenDockerContextIsDefault()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=default" });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(DockerCli.GetCurrentEndpoint("default"), currentEndpoint);
      }

      [Fact]
      public void ReturnsConfiguredEndpointWhenDockerContextIsCustomFromPropertiesFile()
      {
        // Given
        using var context = new ConfigMetaFile("custom", "tcp://127.0.0.1:2375/");

        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=custom", context.GetDockerConfig() });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(new Uri("tcp://127.0.0.1:2375/"), currentEndpoint);
      }

      [Fact]
      public void ReturnsConfiguredEndpointWhenDockerContextIsCustomFromConfigFile()
      {
        // Given
        using var context = new ConfigMetaFile("custom", "tcp://127.0.0.1:2375/");

        // This test reads the current context JSON node from the Docker config file.
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { context.GetDockerConfig() });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(new Uri("tcp://127.0.0.1:2375/"), currentEndpoint);
      }

      [SkipIfHostOrContextIsSet]
      public void ReturnsActiveEndpointWhenDockerContextIsUnset()
      {
        var currentEndpoint = new DockerConfig().GetCurrentEndpoint();
        Assert.Equal(DockerCli.GetCurrentEndpoint(), currentEndpoint);
      }

      [Fact]
      public void ReturnsNullWhenDockerContextNotFound()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=missing" });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Null(currentEndpoint);
      }
    }

    public sealed class DockerHostConfigurationTests
    {
      [Fact]
      public void ReturnsActiveEndpointWhenDockerHostIsEmpty()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=" });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(DockerCli.GetCurrentEndpoint(), currentEndpoint);
      }

      [Fact]
      public void ReturnsConfiguredEndpointWhenDockerHostIsSet()
      {
        // Given
        using var context = new ConfigMetaFile("custom", "");

        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=tcp://127.0.0.1:2375/", context.GetDockerConfig() });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(new Uri("tcp://127.0.0.1:2375/"), currentEndpoint);
      }
    }

    private sealed class SkipIfHostOrContextIsSet : FactAttribute
    {
      public SkipIfHostOrContextIsSet()
      {
        const string reason = "The Docker CLI doesn't know about ~/.testcontainers.properties file.";
        var dockerHost = PropertiesFileConfiguration.Instance.GetDockerHost();
        var dockerContext = PropertiesFileConfiguration.Instance.GetDockerContext();
        Skip = dockerHost != null || dockerContext != null ? reason : string.Empty;
      }
    }

    private sealed class ConfigMetaFile : IDisposable
    {
      private const string ConfigFileJson = "{{\"currentContext\":\"{0}\"}}";

      private const string MetaFileJson = "{{\"Name\":\"{0}\",\"Metadata\":{{}},\"Endpoints\":{{\"docker\":{{\"Host\":\"{1}\",\"SkipTLSVerify\":false}}}}}}";

      private readonly string _dockerConfigDirectoryPath;

      public ConfigMetaFile(string context, string endpoint, [CallerMemberName] string caller = "")
      {
        _dockerConfigDirectoryPath = Path.Combine(TestSession.TempDirectoryPath, caller);
        var dockerContextHash = Convert.ToHexString(SHA256.HashData(Encoding.Default.GetBytes(context))).ToLowerInvariant();
        var dockerContextMetaDirectoryPath = Path.Combine(_dockerConfigDirectoryPath, "contexts", "meta", dockerContextHash);
        _ = Directory.CreateDirectory(dockerContextMetaDirectoryPath);
        File.WriteAllText(Path.Combine(_dockerConfigDirectoryPath, "config.json"), string.Format(ConfigFileJson, context));
        File.WriteAllText(Path.Combine(dockerContextMetaDirectoryPath, "meta.json"), string.Format(MetaFileJson, context, endpoint));
      }

      public string GetDockerConfig()
      {
        return "docker.config=" + _dockerConfigDirectoryPath;
      }

      public void Dispose()
      {
        Directory.Delete(_dockerConfigDirectoryPath, true);
      }
    }
  }
}
