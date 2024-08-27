namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public static class DockerConfigTests
  {
    public sealed class DockerContextConfigurationTests
    {
      [Fact]
      public void ReturnsDefaultOsEndpointWhenDockerContextIsEmpty()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=" });
        var dockerConfig = new DockerConfig(new FileInfo("config.json"), customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(TestcontainersSettings.OS.DockerEndpointAuthConfig.Endpoint, currentEndpoint);
      }

      [Fact]
      public void ReturnsDefaultOsEndpointWhenDockerContextIsDefault()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.context=default" });
        var dockerConfig = new DockerConfig(new FileInfo("config.json"), customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(TestcontainersSettings.OS.DockerEndpointAuthConfig.Endpoint, currentEndpoint);
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
      public void ReturnsDefaultOsEndpointWhenDockerHostIsEmpty()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=" });
        var dockerConfig = new DockerConfig(new FileInfo("config.json"), customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(TestcontainersSettings.OS.DockerEndpointAuthConfig.Endpoint, currentEndpoint);
      }

      [Fact]
      public void ReturnsConfiguredEndpointWhenDockerHostIsSet()
      {
        // Given
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { "docker.host=tcp://127.0.0.1:2375/" });
        var dockerConfig = new DockerConfig(new FileInfo("config.json"), customConfiguration);

        // When
        var currentEndpoint = dockerConfig.GetCurrentEndpoint();

        // Then
        Assert.Equal(new Uri("tcp://127.0.0.1:2375/"), currentEndpoint);
      }
    }
  }
}
