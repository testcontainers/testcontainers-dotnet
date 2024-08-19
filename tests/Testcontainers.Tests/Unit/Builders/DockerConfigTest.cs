namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using Xunit;

  public class DockerConfigTest
  {
    [Fact]
    public void GetCurrentEndpoint()
    {
      var endpoint = DockerConfig.Default.GetCurrentEndpoint();
      Assert.NotNull(endpoint);

      var expectedEndpoint = DockerCli.GetCurrentEndpoint();
      Assert.Equal(expectedEndpoint, endpoint);
    }
  }
}
