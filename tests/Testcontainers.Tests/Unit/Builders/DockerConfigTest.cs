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
        using var context = new ConfigMetaFile("custom", new Uri("tcp://127.0.0.1:2375/"));

        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=custom", $"docker.config={context.DockerConfigDirectoryPath}" });
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
        using var context = new ConfigMetaFile("custom", new Uri("tcp://127.0.0.1:2375/"));

        // This test reads the current context JSON node from the Docker config file.
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { $"docker.config={context.DockerConfigDirectoryPath}" });
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
      public void ThrowsWhenDockerContextNotFound()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=missing" });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var exception = Assert.Throws<DockerConfigurationException>(() => dockerConfig.GetCurrentEndpoint());

        // Then
        Assert.Equal("The Docker context 'missing' does not exist", exception.Message);
        Assert.IsType<DirectoryNotFoundException>(exception.InnerException);
      }

      [Fact]
      public void ThrowsWhenDockerConfigEndpointNotFound()
      {
        // Given
        using var context = new ConfigMetaFile("custom");

        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=custom", $"docker.config={context.DockerConfigDirectoryPath}" });
        var dockerConfig = new DockerConfig(customConfiguration);

        // When
        var exception = Assert.Throws<DockerConfigurationException>(() => dockerConfig.GetCurrentEndpoint());

        // Then
        Assert.StartsWith("The Docker host is null in ", exception.Message);
        Assert.Contains(context.DockerConfigDirectoryPath, exception.Message);
        Assert.EndsWith(" (JSONPath: Endpoints.docker.Host)", exception.Message);
        Assert.Null(exception.InnerException);
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
        using var context = new ConfigMetaFile("custom");

        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=tcp://127.0.0.1:2375/", $"docker.config={context.DockerConfigDirectoryPath}" });
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

      public string DockerConfigDirectoryPath { get; }

      public ConfigMetaFile(string context, [CallerMemberName] string caller = "")
      {
        DockerConfigDirectoryPath = InitializeContext(context, null, caller);
      }

      public ConfigMetaFile(string context, Uri endpoint, [CallerMemberName] string caller = "")
      {
        DockerConfigDirectoryPath = InitializeContext(context, endpoint, caller);
      }

      private static string InitializeContext(string context, Uri endpoint, [CallerMemberName] string caller = "")
      {
        var dockerConfigDirectoryPath = Path.Combine(TestSession.TempDirectoryPath, caller);
        var dockerContextHash = Convert.ToHexString(SHA256.HashData(Encoding.Default.GetBytes(context))).ToLowerInvariant();
        var dockerContextMetaDirectoryPath = Path.Combine(dockerConfigDirectoryPath, "contexts", "meta", dockerContextHash);
        _ = Directory.CreateDirectory(dockerContextMetaDirectoryPath);
        File.WriteAllText(Path.Combine(dockerConfigDirectoryPath, "config.json"), string.Format(ConfigFileJson, context));
        File.WriteAllText(Path.Combine(dockerContextMetaDirectoryPath, "meta.json"), endpoint == null ?  "{}" : string.Format(MetaFileJson, context, endpoint.AbsoluteUri));
        return dockerConfigDirectoryPath;
      }

      public void Dispose()
      {
        Directory.Delete(DockerConfigDirectoryPath, true);
      }
    }
  }
}
