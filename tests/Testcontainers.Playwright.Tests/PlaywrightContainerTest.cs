namespace Testcontainers.Playwright.Tests;

public class PlaywrightContainerTest : IClassFixture<TestInitializer>
{
  private readonly Uri _helloWorldBaseAddress;

  public PlaywrightContainerTest(TestInitializer testInitializer)
  {
    _helloWorldBaseAddress = testInitializer._helloWorldBaseAddress;
  }

  [Fact]
  public async Task HeadingElementReturnsHelloWorld()
  {
    // Given
    var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    var browser = await playwright.Chromium.ConnectAsync($"ws://localhost:63333/playwright");
    var page = await browser.NewPageAsync();

    // When
    await page.GotoAsync(_helloWorldBaseAddress.ToString());
    var headingElement = await page.QuerySelectorAsync("h1");
    var headingElementText = await headingElement.InnerTextAsync();

    // Then
    Assert.Equal("Hello world", headingElementText);
  }
}
