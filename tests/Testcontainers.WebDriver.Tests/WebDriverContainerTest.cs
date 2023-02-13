namespace Testcontainers.WebDriver;

using System;
using Xunit.Abstractions;

public class WebDriverContainerTest : IAsyncLifetime
{
  private readonly ITestOutputHelper testOutputHelper;
  private readonly WebDriverContainer _webDriverContainer = new WebDriverBuilder().Build();

  public WebDriverContainerTest(ITestOutputHelper testOutputHelper)
  {
    this.testOutputHelper = testOutputHelper;
  }

  public Task InitializeAsync()
  {
    return _webDriverContainer.StartAsync();
  }

  public Task DisposeAsync()
  {
    return _webDriverContainer.DisposeAsync().AsTask();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public void CreateContainer()
  {
  }
}
