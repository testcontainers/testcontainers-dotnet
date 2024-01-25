namespace Testcontainers.Ollama.Tests;

public class OllamaContainerTests : IAsyncLifetime
{
  private readonly ITestOutputHelper _testOutputHelper;
  private OllamaContainer _ollamaContainer;

  public OllamaContainerTests(ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  public async Task InitializeAsync()
  {
    TestcontainersSettings.Logger = new TestOutputLogger(nameof(OllamaContainerTests), _testOutputHelper);
    _ollamaContainer = new OllamaBuilder().Build();
    await _ollamaContainer.StartAsync();
  }

  public async Task DisposeAsync()
  {
    await _ollamaContainer.DisposeAsync().AsTask();
  }

  [Fact]
  [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
  public async Task OllamaContainerReturnsSuccessful()
  {
    var client = new OllamaApiClient(_ollamaContainer.GetBaseUrl(), _ollamaContainer.ModelName);

    var chatRequest = new ChatRequest() {
      Model = _ollamaContainer.ModelName,
      Stream = false,
      Messages = new List<Message>()
      {
        new Message() { Content = "You are very helpful", Role = ChatRole.System },
        new Message() { Content = "Hello", Role = ChatRole.User },
      }
    };

    var response = await client.SendChat(chatRequest, stream => { });
    _testOutputHelper.WriteJson(response);

    Assert.True(response.Any());
  }

}
