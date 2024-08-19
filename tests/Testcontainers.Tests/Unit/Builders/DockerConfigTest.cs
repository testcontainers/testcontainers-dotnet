namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public class DockerConfigTest
  {
    [Fact]
    public void GetCurrentEndpoint()
    {
      var endpoint = new DockerConfig().GetCurrentEndpoint();
      Assert.NotNull(endpoint);
    }
  }
}
