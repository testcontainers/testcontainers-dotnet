namespace Testcontainers.WebDriver;

public class WebDriverContainerTest : IAsyncLifetime
{
    private readonly WebDriverContainer _webDriverContainer = new WebDriverBuilder().Build();

    public Task InitializeAsync()
    {
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
    }
}
