namespace DotNet.Testcontainers.Tests.Unit
{
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using Xunit;
  using Xunit.Abstractions;

  public class DockerConfigTest(ITestOutputHelper output)
  {
    [Fact]
    public void GetCurrentEndpoint()
    {
      var expectedEndpoint = DockerCli.GetCurrentEndpoint();
      var endpoint = DockerConfig.Default.GetCurrentEndpoint();
      output.WriteLine($"DockerConfig.Default.GetCurrentEndpoint() => {endpoint}");
      Assert.Equal(expectedEndpoint, endpoint);
    }
  }
}
