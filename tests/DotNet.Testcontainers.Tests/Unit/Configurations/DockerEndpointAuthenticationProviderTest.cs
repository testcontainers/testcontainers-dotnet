namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  [CollectionDefinition(nameof(DockerEndpointAuthenticationProviderTest), DisableParallelization = true)]
  public sealed class DockerEndpointAuthenticationProviderTest
  {
    [Collection(nameof(DockerEndpointAuthenticationProviderTest))]
    public sealed class EnvironmentEndpointAuthenticationProviderTest : IDisposable
    {
      private const string DockerHost = "127.0.0.1:2375";

      public EnvironmentEndpointAuthenticationProviderTest()
      {
        Environment.SetEnvironmentVariable("DOCKER_HOST", DockerHost);
      }

      [Fact]
      public void GetAuthConfig()
      {
        using (var clientConfiguration = new DockerEndpointAuthenticationProvider().GetAuthConfig()!.GetDockerClientConfiguration())
        {
          Assert.Equal(DockerHost, clientConfiguration.EndpointBaseUri.ToString());
        }
      }

      public void Dispose()
      {
        Environment.SetEnvironmentVariable("DOCKER_HOST", null);
      }
    }
  }
}
