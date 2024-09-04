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
      public void ReturnsConfiguredEndpointWhenDockerContextIsCustomFromProperties()
      {
        // Given
        using var context = new TempDockerContext("custom", "tcp://127.0.0.1:2375/");
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=custom" });
        var dockerConfig = new DockerConfig(context.ConfigFile, customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(new Uri("tcp://127.0.0.1:2375/"), currentEndpoint);
      }

      [Fact]
      public void ReturnsConfiguredEndpointWhenDockerContextIsCustomFromConfigFile()
      {
        // Given
        using var context = new TempDockerContext("custom", "tcp://127.0.0.1:2375/");
        var dockerConfig = new DockerConfig(context.ConfigFile);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(new Uri("tcp://127.0.0.1:2375/"), currentEndpoint);
      }

      [Fact]
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
        using var context = new TempDockerContext("custom", "");
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=tcp://127.0.0.1:2375/" });
        var dockerConfig = new DockerConfig(context.ConfigFile, customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(new Uri("tcp://127.0.0.1:2375/"), currentEndpoint);
      }
    }

    private class TempDockerContext : IDisposable
    {
      private readonly DirectoryInfo _directory;

      public TempDockerContext(string contextName, string endpoint, [CallerMemberName] string caller = "")
      {
        _directory = new DirectoryInfo(Path.Combine(TestSession.TempDirectoryPath, caller));
        _directory.Create();
        ConfigFile = new FileInfo(Path.Combine(_directory.FullName, "config.json"));

        // lang=json
        var config = $$"""{ "currentContext": "{{contextName}}" }""";
        File.WriteAllText(ConfigFile.FullName, config);

        // lang=json
        var meta = $$"""{ "Name": "{{contextName}}", "Metadata": {}, "Endpoints": { "docker": { "Host": "{{endpoint}}", "SkipTLSVerify":false } } }""";
        var dockerContextHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(contextName))).ToLowerInvariant();
        var metaDirectory = Path.Combine(_directory.FullName, "contexts", "meta", dockerContextHash);
        Directory.CreateDirectory(metaDirectory);
        File.WriteAllText(Path.Combine(metaDirectory, "meta.json"), meta);
      }

      public FileInfo ConfigFile { get; }

      public void Dispose()
      {
        _directory.Delete(recursive: true);
      }
    }
  }
}
