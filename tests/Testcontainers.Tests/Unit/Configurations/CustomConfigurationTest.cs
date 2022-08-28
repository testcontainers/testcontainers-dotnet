namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  public static class CustomConfigurationTest
  {
    [CollectionDefinition(nameof(EnvironmentConfigurationTest), DisableParallelization = true)]
    [Collection(nameof(EnvironmentConfigurationTest))]
    public sealed class EnvironmentConfigurationTest : IDisposable
    {
      private static readonly IList<string> EnvironmentVariables = new List<string>();

      static EnvironmentConfigurationTest()
      {
        EnvironmentVariables.Add("DOCKER_CONFIG");
        EnvironmentVariables.Add("DOCKER_HOST");
        EnvironmentVariables.Add("DOCKER_AUTH_CONFIG");
        EnvironmentVariables.Add("TESTCONTAINERS_RYUK_DISABLED");
        EnvironmentVariables.Add("TESTCONTAINERS_RYUK_CONTAINER_IMAGE");
        EnvironmentVariables.Add("TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX");
      }

      [Theory]
      [InlineData("", "", null)]
      [InlineData("DOCKER_CONFIG", "", null)]
      [InlineData("DOCKER_CONFIG", "~/.docker/", "~/.docker/")]
      public void GetDockerConfigCustomConfiguration(string propertyName, string propertyValue, string expected)
      {
        SetEnvironmentVariable(propertyName, propertyValue);
        ICustomConfiguration customConfiguration = new EnvironmentConfiguration();
        Assert.Equal(expected, customConfiguration.GetDockerConfig());
      }

      [Theory]
      [InlineData("", "", null)]
      [InlineData("DOCKER_HOST", "", null)]
      [InlineData("DOCKER_HOST", "tcp://127.0.0.1:2375/", "tcp://127.0.0.1:2375/")]
      public void GetDockerHostCustomConfiguration(string propertyName, string propertyValue, string expected)
      {
        SetEnvironmentVariable(propertyName, propertyValue);
        ICustomConfiguration customConfiguration = new EnvironmentConfiguration();
        Assert.Equal(expected, customConfiguration.GetDockerHost()?.ToString());
      }

      [Theory]
      [InlineData("", "", null)]
      [InlineData("DOCKER_AUTH_CONFIG", "", null)]
      [InlineData("DOCKER_AUTH_CONFIG", "{jsonReaderException}", null)]
      [InlineData("DOCKER_AUTH_CONFIG", "{}", "{}")]
      [InlineData("DOCKER_AUTH_CONFIG", "{\"auths\":null}", "{\"auths\":null}")]
      [InlineData("DOCKER_AUTH_CONFIG", "{\"auths\":{}}", "{\"auths\":{}}")]
      [InlineData("DOCKER_AUTH_CONFIG", "{\"auths\":{\"ghcr.io\":{}}}", "{\"auths\":{\"ghcr.io\":{}}}")]
      public void GetDockerAuthConfigCustomConfiguration(string propertyName, string propertyValue, string expected)
      {
        SetEnvironmentVariable(propertyName, propertyValue);
        ICustomConfiguration customConfiguration = new EnvironmentConfiguration();
        Assert.Equal(expected, customConfiguration.GetDockerAuthConfig()?.RootElement.ToString());
      }

      [Theory]
      [InlineData("", "", false)]
      [InlineData("TESTCONTAINERS_RYUK_DISABLED", "", false)]
      [InlineData("TESTCONTAINERS_RYUK_DISABLED", "false", false)]
      [InlineData("TESTCONTAINERS_RYUK_DISABLED", "true", true)]
      public void GetRyukDisabledCustomConfiguration(string propertyName, string propertyValue, bool expected)
      {
        SetEnvironmentVariable(propertyName, propertyValue);
        ICustomConfiguration customConfiguration = new EnvironmentConfiguration();
        Assert.Equal(expected, customConfiguration.GetRyukDisabled());
      }

      [Theory]
      [InlineData("", "", null)]
      [InlineData("TESTCONTAINERS_RYUK_CONTAINER_IMAGE", "", null)]
      [InlineData("TESTCONTAINERS_RYUK_CONTAINER_IMAGE", "alpine:latest", "alpine:latest")]
      public void GetRyukContainerImageCustomConfiguration(string propertyName, string propertyValue, string expected)
      {
        SetEnvironmentVariable(propertyName, propertyValue);
        ICustomConfiguration customConfiguration = new EnvironmentConfiguration();
        Assert.Equal(expected, customConfiguration.GetRyukContainerImage()?.FullName);
      }

      [Theory]
      [InlineData("", "", null)]
      [InlineData("TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX", "", null)]
      [InlineData("TESTCONTAINERS_HUB_IMAGE_NAME_PREFIX", "my.proxy.com", "my.proxy.com")]
      public void GetHubImageNamePrefixCustomConfiguration(string propertyName, string propertyValue, string expected)
      {
        SetEnvironmentVariable(propertyName, propertyValue);
        ICustomConfiguration customConfiguration = new EnvironmentConfiguration();
        Assert.Equal(expected, customConfiguration.GetHubImageNamePrefix());
      }

      public void Dispose()
      {
        foreach (var propertyName in EnvironmentVariables)
        {
          SetEnvironmentVariable(propertyName, null);
        }
      }

      private static void SetEnvironmentVariable(string propertyName, string propertyValue)
      {
        if (!string.IsNullOrEmpty(propertyName))
        {
          Environment.SetEnvironmentVariable(propertyName, propertyValue);
        }
      }
    }

    public sealed class PropertiesFileConfigurationTest
    {
      [Theory]
      [InlineData("", null)]
      [InlineData("docker.config=", null)]
      [InlineData("docker.config=~/.docker/", "~/.docker/")]
      public void GetDockerConfigCustomConfiguration(string configuration, string expected)
      {
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { configuration });
        Assert.Equal(expected, customConfiguration.GetDockerConfig());
      }

      [Theory]
      [InlineData("", null)]
      [InlineData("docker.host=", null)]
      [InlineData("docker.host=tcp://127.0.0.1:2375/", "tcp://127.0.0.1:2375/")]
      public void GetDockerHostCustomConfiguration(string configuration, string expected)
      {
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { configuration });
        Assert.Equal(expected, customConfiguration.GetDockerHost()?.ToString());
      }

      [Theory]
      [InlineData("", null)]
      [InlineData("docker.auth.config=", null)]
      [InlineData("docker.auth.config={jsonReaderException}", null)]
      [InlineData("docker.auth.config={}", "{}")]
      [InlineData("docker.auth.config={\"auths\":null}", "{\"auths\":null}")]
      [InlineData("docker.auth.config={\"auths\":{}}", "{\"auths\":{}}")]
      [InlineData("docker.auth.config={\"auths\":{\"ghcr.io\":{}}}", "{\"auths\":{\"ghcr.io\":{}}}")]
      public void GetDockerAuthConfigCustomConfiguration(string configuration, string expected)
      {
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { configuration });
        Assert.Equal(expected, customConfiguration.GetDockerAuthConfig()?.RootElement.ToString());
      }

      [Theory]
      [InlineData("", false)]
      [InlineData("ryuk.disabled=", false)]
      [InlineData("ryuk.disabled=false", false)]
      [InlineData("ryuk.disabled=true", true)]
      public void GetRyukDisabledCustomConfiguration(string configuration, bool expected)
      {
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { configuration });
        Assert.Equal(expected, customConfiguration.GetRyukDisabled());
      }

      [Theory]
      [InlineData("", null)]
      [InlineData("ryuk.container.image=", null)]
      [InlineData("ryuk.container.image=alpine:latest", "alpine:latest")]
      public void GetRyukContainerImageCustomConfiguration(string configuration, string expected)
      {
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { configuration });
        Assert.Equal(expected, customConfiguration.GetRyukContainerImage()?.FullName);
      }

      [Theory]
      [InlineData("", null)]
      [InlineData("hub.image.name.prefix=", null)]
      [InlineData("hub.image.name.prefix=my.proxy.com", "my.proxy.com")]
      public void GetHubImageNamePrefixCustomConfiguration(string configuration, string expected)
      {
        ICustomConfiguration customConfiguration = new PropertiesFileConfiguration(new[] { configuration });
        Assert.Equal(expected, customConfiguration.GetHubImageNamePrefix());
      }
    }
  }
}
