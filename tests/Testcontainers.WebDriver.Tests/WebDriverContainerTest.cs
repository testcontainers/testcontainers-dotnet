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

    public sealed class HelloWorldContainer : IClassFixture<WebDriverContainerTest>, IAsyncLifetime
    {
        private const string HelloWorldImage = "testcontainers/helloworld:1.1.0";

        private readonly Uri _helloWorldEndpoint = new UriBuilder(Uri.UriSchemeHttp, "hello-world-container", 8080).Uri;

        private readonly IContainer _helloWorldContainer;

        private readonly WebDriverContainerTest _fixture;

        public HelloWorldContainer(WebDriverContainerTest fixture)
        {
            _fixture = fixture;

            // TODO: Pass the depended container (Docker resource) to the builder and resolve the dependency graph internal.
            _helloWorldContainer = new ContainerBuilder()
                .WithImage(HelloWorldImage)
                .WithNetwork(_fixture._webDriverContainer.GetNetwork())
                .WithNetworkAliases(_helloWorldEndpoint.Host)
                .WithPortBinding(_helloWorldEndpoint.Port, true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                    request.ForPath("/").ForPort(Convert.ToUInt16(_helloWorldEndpoint.Port))))
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
        public async Task HeadingElementReturnsHelloWorld()
        {
            // Given
            using var driver = new RemoteWebDriver(new Uri(_fixture._webDriverContainer.GetConnectionString()), new ChromeOptions());

            var videoFilePath = Path.Combine(TestSession.TempDirectoryPath, Path.GetRandomFileName());

            // When
            driver.Navigate().GoToUrl(_helloWorldEndpoint.ToString());
            var headingElementText = driver.FindElementByTagName("h1").Text;

            await _fixture._webDriverContainer.StopAsync()
                .ConfigureAwait(false);

            await _fixture._webDriverContainer.ExportVideoAsync(videoFilePath)
                .ConfigureAwait(false);

            // Then
            Assert.Equal("Hello world", headingElementText);
            Assert.True(File.Exists(videoFilePath));
        }
    }
}