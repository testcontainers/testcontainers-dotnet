namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Containers.Configurations;
  using Xunit;

  public class DockerClientAuthenticationConfigurationTest
  {
    // TODO: Add all tests, organize them well and clean them up.
    [Fact]
    public void Foo()
    {
      var clientAuthConfig = new DockerClientEnvironmentAuthenticationConfiguration();
      Assert.True(clientAuthConfig.IsApplicable);
    }

    [Fact]
    public void Bar()
    {
      const string dockerHost = "tcp://127.0.0.1:1337/";
      Environment.SetEnvironmentVariable("DOCKER_HOST", dockerHost, EnvironmentVariableTarget.Process);
      var clientAuthConfig = new DockerClientEnvironmentAuthenticationConfiguration();
      Assert.Equal(dockerHost, clientAuthConfig.Endpoint.AbsoluteUri);
      Assert.True(clientAuthConfig.IsApplicable);
    }

    [Fact]
    public void IsNotIsApplicable()
    {
      Assert.False(new DockerClientAuthenticationConfiguration(null).IsApplicable);
      Assert.False(new DockerClientEnvironmentAuthenticationConfiguration(null, null, false).IsApplicable);
    }

    [Fact]
    public void HasRequiredTlsCertificates()
    {
      var clientAuthConfig = new DockerClientEnvironmentAuthenticationConfiguration(null, Path.Combine("assets", "tls"), true);
      Assert.True(clientAuthConfig.IsTlsVerificationEnabled);
    }

    [Fact]
    public void DoesNotSupportTlsCertificates()
    {
      var clientAuthConfig = new DockerClientAuthenticationConfiguration(null);
      Assert.False(clientAuthConfig.IsTlsVerificationEnabled);
    }

    [Fact]
    public void DoesNotHaveRequiredTlsCertificates()
    {
      var clientAuthConfig = new DockerClientEnvironmentAuthenticationConfiguration(null, Path.Combine("assets"), true);
      Assert.False(clientAuthConfig.IsTlsVerificationEnabled);
    }
  }
}
