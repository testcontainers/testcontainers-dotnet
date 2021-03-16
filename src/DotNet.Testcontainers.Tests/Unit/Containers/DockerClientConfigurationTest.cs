namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System;
  using Testcontainers.Containers.Configurations;
  using Xunit;

  public class DockerClientConfigurationTest
  {
    // TODO: Add all tests, organize them well and clean them up.
    [Fact]
    public void Foo()
    {
      var clientAuthConfig = new DockerClientEnvironmentConfiguration();
      Assert.True(clientAuthConfig.IsApplicable);
    }

    [Fact]
    public void Bar()
    {
      const string dockerHost = "tcp://127.0.0.1:1337/";
      Environment.SetEnvironmentVariable("DOCKER_HOST", dockerHost, EnvironmentVariableTarget.Process);
      var clientAuthConfig = new DockerClientEnvironmentConfiguration();
      Assert.Equal(dockerHost, clientAuthConfig.Endpoint.AbsoluteUri);
      Assert.True(clientAuthConfig.IsApplicable);
    }

    [Fact]
    public void IsNotIsApplicable()
    {
      Assert.False(new DockerClientConfiguration(null).IsApplicable);
      Assert.False(new DockerClientEnvironmentConfiguration(null, null, false).IsApplicable);
    }
  }
}
