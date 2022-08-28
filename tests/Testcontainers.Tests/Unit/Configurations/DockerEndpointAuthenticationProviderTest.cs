namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using Xunit;

  [CollectionDefinition(nameof(DockerEndpointAuthenticationProviderTest), DisableParallelization = true)]
  [Collection(nameof(DockerEndpointAuthenticationProviderTest))]
  public sealed class DockerEndpointAuthenticationProviderTest
  {
    [Theory]
    [ClassData(typeof(AuthConfigTestData))]
    public void GetDockerClientConfiguration(IDockerEndpointAuthenticationConfiguration authConfig, Uri expectedDockerClientEndpoint)
    {
      using (var dockerClientConfiguration = authConfig.GetDockerClientConfiguration())
      {
        Assert.Equal(expectedDockerClientEndpoint, authConfig.Endpoint);
        Assert.Equal(expectedDockerClientEndpoint, dockerClientConfiguration.EndpointBaseUri);
      }
    }

    private sealed class AuthConfigTestData : List<object[]>
    {
      public AuthConfigTestData()
      {
        const string dockerHost = "tcp://127.0.0.1:2375";
        Environment.SetEnvironmentVariable("DOCKER_HOST", dockerHost);
        this.Add(new object[] { new EnvironmentEndpointAuthenticationProvider().GetAuthConfig(), new Uri(dockerHost) });
        this.Add(new object[] { new NpipeEndpointAuthenticationProvider().GetAuthConfig(), new Uri("npipe://./pipe/docker_engine") });
        this.Add(new object[] { new UnixEndpointAuthenticationProvider().GetAuthConfig(), new Uri("unix:/var/run/docker.sock") });
        Environment.SetEnvironmentVariable("DOCKER_HOST", null);
      }
    }
  }
}
