namespace Testcontainers.WebDriver;

public abstract class WebDriverContainerTest : IAsyncLifetime
{
    private readonly Uri _helloWorldBaseAddress = new UriBuilder(
        Uri.UriSchemeHttp,
        "hello-world-container",
        8080
    ).Uri;

    private readonly IContainer _helloWorldContainer;

    private readonly WebDriverContainer _webDriverContainer;

    private WebDriverContainerTest(WebDriverContainer webDriverContainer)
    {
        _helloWorldContainer = new ContainerBuilder()
            .WithImage(CommonImages.HelloWorld)
            .WithNetwork(webDriverContainer.GetNetwork())
            .WithNetworkAliases(_helloWorldBaseAddress.Host)
            .WithPortBinding(_helloWorldBaseAddress.Port, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(request =>
                        request.ForPath("/").ForPort(Convert.ToUInt16(_helloWorldBaseAddress.Port))
                    )
            )
            .Build();

        _webDriverContainer = webDriverContainer;
    }

    public async ValueTask InitializeAsync()
    {
        await _webDriverContainer.StartAsync().ConfigureAwait(false);

        await _helloWorldContainer.StartAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void HeadingElementReturnsHelloWorld()
    {
        // Given
        using var driver = new RemoteWebDriver(
            new Uri(_webDriverContainer.GetConnectionString()),
            new ChromeOptions()
        );

        // When
        driver.Navigate().GoToUrl(_helloWorldBaseAddress.ToString());
        var headingElementText = driver.FindElementByTagName("h1").Text;

        // Then
        Assert.Equal("Hello world", headingElementText);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _helloWorldContainer.DisposeAsync().ConfigureAwait(false);

        await _webDriverContainer.DisposeAsync().ConfigureAwait(false);
    }

    [UsedImplicitly]
    public sealed class RecordingEnabled : WebDriverContainerTest
    {
        public RecordingEnabled()
            : base(new WebDriverBuilder().WithRecording().Build()) { }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task ExportVideoWritesFile()
        {
            // Given
            var videoFilePath = Path.Combine(
                TestSession.TempDirectoryPath,
                Path.GetRandomFileName()
            );

            // When
            HeadingElementReturnsHelloWorld();

            await _webDriverContainer
                .StopAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            await _webDriverContainer
                .ExportVideoAsync(videoFilePath, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.True(File.Exists(videoFilePath));
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public Task ExportVideoThrowsInvalidOperationException()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                _webDriverContainer.ExportVideoAsync(
                    string.Empty,
                    TestContext.Current.CancellationToken
                )
            );
        }
    }

    [UsedImplicitly]
    public sealed class RecordingDisabled : WebDriverContainerTest
    {
        public RecordingDisabled()
            : base(new WebDriverBuilder().Build()) { }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public Task ExportVideoThrowsInvalidOperationException()
        {
            return Assert.ThrowsAsync<InvalidOperationException>(() =>
                _webDriverContainer.ExportVideoAsync(
                    string.Empty,
                    TestContext.Current.CancellationToken
                )
            );
        }
    }
}
