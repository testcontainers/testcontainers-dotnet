using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Testcontainers.Playwright.Tests
{
  public class TestInitializer : IAsyncLifetime
  {
    internal readonly Uri _helloWorldBaseAddress = new UriBuilder(Uri.UriSchemeHttp, "hello-world-container", 8080).Uri;
    private IContainer _helloWorldContainer;
    private PlaywrightContainer _playwrightContainer;

    private const string PlaywrightContainerName = "testplaywright";

    public async Task InitializeAsync()
    {
      var network = CreateNetwork();

      _helloWorldContainer = CreateHelloWorldContainer(network);
      _playwrightContainer = CreatePlaywrightContainer(network);

      await StartContainersAsync();
    }

    public async Task DisposeAsync()
    {
      await DisposeContainersAsync();
    }

    private INetwork CreateNetwork()
    {
      return new NetworkBuilder()
        .WithName(Guid.NewGuid().ToString("D"))
        .WithDriver(NetworkDriver.Bridge)
        .WithCleanUp(true)
        .Build();
    }

    private IContainer CreateHelloWorldContainer(INetwork network)
    {
      return new ContainerBuilder()
        .WithImage("testcontainers/helloworld:1.1.0")
        .WithNetwork(network)
        .WithNetworkAliases(_helloWorldBaseAddress.Host)
        .WithPortBinding(_helloWorldBaseAddress.Port, true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
          request.ForPath("/").ForPort(Convert.ToUInt16(_helloWorldBaseAddress.Port))))
        .Build();
    }

    private PlaywrightContainer CreatePlaywrightContainer(INetwork network)
    {
      return new PlaywrightBuilder()
        .WithNetwork(network)
        .WithName(PlaywrightContainerName)
        .WithNetworkAliases(PlaywrightContainerName)
        .WithPortBinding(63333, 53333)
        .WithBrowser(PlaywrightBrowser.Chromium)
        .Build();
    }

    private async Task StartContainersAsync()
    {
      await _helloWorldContainer.StartAsync();
      await _playwrightContainer.StartAsync();
    }

    private async Task DisposeContainersAsync()
    {
      await _playwrightContainer.DisposeAsync();
      await _helloWorldContainer.DisposeAsync();
    }
  }
}
