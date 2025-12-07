namespace Testcontainers.Playwright;

public abstract class PlaywrightContainerTest : IAsyncLifetime
{
    private readonly Uri _helloWorldBaseAddress = new UriBuilder(Uri.UriSchemeHttp, "hello-world-container", 8080).Uri;

    private readonly IContainer _helloWorldContainer;

    private readonly PlaywrightContainer _playwrightContainer;

    private PlaywrightContainerTest(PlaywrightContainer playwrightContainer)
    {
        _helloWorldContainer = new ContainerBuilder(CommonImages.HelloWorld)
            .WithNetwork(playwrightContainer.GetNetwork())
            .WithNetworkAliases(_helloWorldBaseAddress.Host)
            .WithPortBinding(_helloWorldBaseAddress.Port, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath("/").ForPort(Convert.ToUInt16(_helloWorldBaseAddress.Port))))
            .Build();

        _playwrightContainer = playwrightContainer;
    }

    public async ValueTask InitializeAsync()
    {
        await _playwrightContainer.StartAsync()
            .ConfigureAwait(false);

        await _helloWorldContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    // # --8<-- [start:UsePlaywrightContainer]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HeadingElementReturnsHelloWorld()
    {
        // Given
        var playwright = await Microsoft.Playwright.Playwright.CreateAsync()
            .ConfigureAwait(true);

        var browser = await playwright.Chromium.ConnectAsync(_playwrightContainer.GetConnectionString())
            .ConfigureAwait(true);

        var page = await browser.NewPageAsync()
            .ConfigureAwait(true);

        // When
        await page.GotoAsync(_helloWorldBaseAddress.ToString())
            .ConfigureAwait(true);

        var headingElement = await page.QuerySelectorAsync("h1")
            .ConfigureAwait(true);

        var headingElementText = await headingElement!.InnerTextAsync()
            .ConfigureAwait(true);

        // Then
        Assert.Equal("Hello world", headingElementText);
    }
    // # --8<-- [end:UsePlaywrightContainer]

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _helloWorldContainer.DisposeAsync()
            .ConfigureAwait(false);

        await _playwrightContainer.DisposeAsync()
            .ConfigureAwait(false);
    }

    [UsedImplicitly]
    public sealed class PlaywrightDefaultConfiguration : PlaywrightContainerTest
    {
        public PlaywrightDefaultConfiguration()
            : base(new PlaywrightBuilder(TestSession.GetImageFromDockerfile()).Build())
        {
        }
    }
}