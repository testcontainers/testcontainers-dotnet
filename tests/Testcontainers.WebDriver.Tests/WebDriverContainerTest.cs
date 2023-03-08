namespace Testcontainers.WebDriver;

public class WebDriverContainerTest : IAsyncLifetime
{
  private const ushort UiPort = 8080;
  private const string NetworkAliasName = "helloworld";

  private static readonly INetwork Network = new NetworkBuilder()
    .WithDriver(NetworkDriver.Bridge)
    .Build();

  private readonly WebDriverContainer webDriverContainer = new WebDriverBuilder()
    .WithBrowser(WebDriverBrowser.Firefox)
    .WithNetwork(Network)
    .Build();

  private readonly IContainer helloWorldTestContainer = new ContainerBuilder()
    .WithImage("testcontainers/helloworld")
    .WithPortBinding(UiPort, UiPort)
    .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(UiPort))
    .WithNetwork(Network)
    .WithNetworkAliases(NetworkAliasName)
    .Build();

  public async Task InitializeAsync()
  {
    await Network.CreateAsync()
      .ConfigureAwait(false);

    await this.helloWorldTestContainer.StartAsync()
      .ConfigureAwait(false);

    await this.webDriverContainer.StartAsync()
      .ConfigureAwait(false);
  }

  public async Task DisposeAsync()
  {
    await this.webDriverContainer.DisposeAsync()
      .ConfigureAwait(false);

    await this.helloWorldTestContainer.DisposeAsync()
      .ConfigureAwait(false);

    await Network.DeleteAsync()
      .ConfigureAwait(false);
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public void CreateContainer()
  {
    // Given
    var driver = new RemoteWebDriver(this.webDriverContainer.GetWebDriverUri(), new FirefoxOptions());

    // Then
    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
    driver.Navigate().GoToUrl($"{NetworkAliasName}:{UiPort}");
    var title = driver.FindElementByTagName("h1").Text;

    // Then
    Assert.Equal("Hello world", title);
  }
}
