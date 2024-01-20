namespace Testcontainers.WebDriver;

public abstract class WebDriverContainerTest : IAsyncLifetime
{
    private readonly Uri _helloWorldBaseAddress = new UriBuilder(Uri.UriSchemeHttp, "hello-world-container", 8080).Uri;

    private readonly IContainer _helloWorldContainer;

    private readonly WebDriverContainer _webDriverContainer;

    private WebDriverContainerTest(WebDriverContainer webDriverContainer)
    {
        _helloWorldContainer = new ContainerBuilder()
            .WithImage("testcontainers/helloworld:1.1.0")
            .WithNetwork(webDriverContainer.GetNetwork())
            .WithNetworkAliases(_helloWorldBaseAddress.Host)
            .WithPortBinding(_helloWorldBaseAddress.Port, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(Convert.ToUInt16(_helloWorldBaseAddress.Port))))
            .Build();

        _webDriverContainer = webDriverContainer;
    }

    public async Task InitializeAsync()
    {
        await _webDriverContainer.StartAsync()
            .ConfigureAwait(false);

        await _helloWorldContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async Task DisposeAsync()
    {
        await _helloWorldContainer.DisposeAsync()
            .ConfigureAwait(false);

        await _webDriverContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [Fact]
    public void HeadingElementReturnsHelloWorld()
    {
        // Given
        using var driver = new RemoteWebDriver(new Uri(_webDriverContainer.GetConnectionString()), new ChromeOptions());

        // When
        driver.Navigate().GoToUrl(_helloWorldBaseAddress.ToString());
        var headingElementText = driver.FindElementByTagName("h1").Text;

        // Then
        Assert.Equal("Hello world", headingElementText);
    }

    [UsedImplicitly]
    public sealed class RecordingEnabled : WebDriverContainerTest
    {
        public RecordingEnabled()
            : base(new WebDriverBuilder().WithRecording().Build())
        {
        }

        [Fact]
        public async Task ExportVideoWritesFile()
        {
            // Given
            var videoFilePath = Path.Combine(TestSession.TempDirectoryPath, Path.GetRandomFileName());

            // When
            await _webDriverContainer.StopAsync()
                .ConfigureAwait(true);

            await _webDriverContainer.ExportVideoAsync(videoFilePath)
                .ConfigureAwait(true);

            // Then
            Assert.True(File.Exists(videoFilePath));
        }

        [Fact]
        public Task ExportVideoThrowsInvalidOperationException()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => _webDriverContainer.ExportVideoAsync(string.Empty));
        }
    }

    [UsedImplicitly]
    public sealed class RecordingDisabled : WebDriverContainerTest
    {
        public RecordingDisabled()
            : base(new WebDriverBuilder().Build())
        {
        }

        [Fact]
        public Task ExportVideoThrowsInvalidOperationException()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() => _webDriverContainer.ExportVideoAsync(string.Empty));
        }
    }
}