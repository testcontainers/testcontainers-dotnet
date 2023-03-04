namespace Testcontainers.WebDriver;

public sealed class WebDriverContainerTest : IAsyncLifetime
{
  private readonly WebDriverContainer _webDriverContainer = new WebDriverBuilder()
    .WithRecording(videosFolder: "C:\\Projects\\selenium-docker\\videos\\recording\\")
    .Build();



  public async Task InitializeAsync()
  {
    await _webDriverContainer.StartAsync()
      .ConfigureAwait(false);
  }

  public async Task DisposeAsync()
  {
    await _webDriverContainer.StopAsync()
      .ConfigureAwait(false);

    await _webDriverContainer.DisposeAsync()
      .ConfigureAwait(false);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public void ConnectionStateReturnsOpen()
  {
    // Given
    var remoteWebDriver = new RemoteWebDriver(_webDriverContainer.GetWebDriverUri(), new ChromeOptions());

    // When
    remoteWebDriver.ExecuteAsyncScript("return document.readyState").Equals("complete");

    // Then
    Assert.Equal(remoteWebDriver.Url, _webDriverContainer.GetWebDriverUri().ToString());
  }
}
