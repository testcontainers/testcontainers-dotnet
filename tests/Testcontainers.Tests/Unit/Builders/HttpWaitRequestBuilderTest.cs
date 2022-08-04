namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Net.Http;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using Moq;
  using Xunit;

  public class HttpWaitRequestBuilderTest
  {
    private const int DefaultPort = 80;
    private static readonly HttpWaitRequest WaitRequest = HttpWaitRequest.ForPort(DefaultPort).Build();

    [Fact]
    public void BuilderIncludeDefaults()
    {
      Assert.Equal("/",  WaitRequest.Path);
      Assert.Equal(HttpMethod.Get, WaitRequest.Method);
      Assert.Empty(WaitRequest.StatusCodes);
      Assert.Equal(1, WaitRequest.ReadTimeout.Seconds);
    }

    [Fact]
    public void WaitForHttpRequestAddsCustomWaitStrategy()
    {
      var container = new Mock<IWaitForContainerOS>();

      container.Object.UntilHttpRequestIsCompleted(WaitRequest);

      container.Verify(c => c.AddCustomWaitStrategy(It.IsAny<UntilHttpRequestIsCompleted>()), Times.Once);
    }
  }
}
