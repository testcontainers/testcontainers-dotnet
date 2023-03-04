namespace Testcontainers.WebDriver;

public class WebDriverContainerTest : IAsyncLifetime
{
  private readonly WebDriverContainer _webDriverContainer = new WebDriverBuilder()
    .WithBrowser(WebDriverType.Chrome)
    .Build();

  private const ushort UiPort = 8080;
  private readonly IContainer _testcontainersHelloworld = new ContainerBuilder()
    .WithImage("testcontainers/helloworld")
    .WithPortBinding(UiPort, true)
    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(UiPort))
    .Build();

  public Task InitializeAsync()
  {
    this._testcontainersHelloworld.StartAsync();
    return this._webDriverContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return this._webDriverContainer.DisposeAsync().AsTask();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public void CreateContainer()
  {
    // Given
    var remoteWebDriver = new RemoteWebDriver(this._webDriverContainer.GetWebDriverUri(), new ChromeOptions());

    // Then
    var helloWorldUri = new UriBuilder(Uri.UriSchemeHttp,
      this._testcontainersHelloworld.Hostname,
          this._testcontainersHelloworld.GetMappedPublicPort(UiPort)).Uri;

    remoteWebDriver.Navigate().GoToUrl(helloWorldUri);
    var title = remoteWebDriver.FindElementByTagName("h1").Text;

    // Then
    Assert.Equal("Hello world", title);
  }
}
