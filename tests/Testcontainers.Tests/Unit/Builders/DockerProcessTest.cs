namespace DotNet.Testcontainers.Tests.Unit
{
  using Xunit;

  public class DockerProcessTest
  {
    [Fact]
    public void GetCurrentEndpoint()
    {
      var endpoint = Builders.DockerProcess.GetCurrentEndpoint();
      Assert.NotNull(endpoint);
    }
  }
}
