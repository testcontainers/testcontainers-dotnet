namespace Testcontainers.WebDriver;

[UsedImplicitly]
public sealed class WebDriverContainerTest : IAsyncLifetime
{
    private readonly WebDriverContainer _webDriverContainer = new WebDriverBuilder().WithRecording().Build();

    public Task InitializeAsync()
    {
        return _webDriverContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _webDriverContainer.DisposeAsync().AsTask();
    }

    public sealed class MyClass : IClassFixture<WebDriverContainerTest>, IAsyncLifetime
    {
        private const string HelloWorldNetworkAlias = "hello-world-container";

        private const string HelloWorldImage = "testcontainers/helloworld:1.1.0";

        private const ushort HelloWorldPort = 8080;

        private readonly Uri _seleniumGridUri;

        private readonly IContainer _helloWorldContainer;

        public MyClass(WebDriverContainerTest fixture)
        {
            _seleniumGridUri = fixture._webDriverContainer.GetWebDriverUri();

            _helloWorldContainer = new ContainerBuilder()
                .WithImage(HelloWorldImage)
                .WithNetwork(fixture._webDriverContainer.GetNetwork())
                .WithNetworkAliases(HelloWorldNetworkAlias)
                .WithPortBinding(HelloWorldPort, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/").ForPort(HelloWorldPort)))
                .Build();
        }

        public Task InitializeAsync()
        {
            return _helloWorldContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return _helloWorldContainer.DisposeAsync().AsTask();
        }

        [Fact]
        public void GetHelloWorld()
        {
            // Given
            using var driver = new RemoteWebDriver(_seleniumGridUri,  new ChromeOptions());

            // When
            driver.Navigate().GoToUrl(new UriBuilder(Uri.UriSchemeHttp, HelloWorldNetworkAlias, HelloWorldPort).ToString());

            // Then
            Assert.Equal("Hello world", driver.FindElementByTagName("h1").Text);
        }
    }
}