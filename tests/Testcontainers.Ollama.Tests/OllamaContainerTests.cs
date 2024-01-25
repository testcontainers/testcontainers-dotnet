namespace Testcontainers.Ollama.Tests
{
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
      await _ollamaContainer.StartOllamaAsync();
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
          new Message() { Content = "What is a name", Role = ChatRole.User },
        }
      };

      var response = await client.SendChat(chatRequest, stream => { });
      response = response.ToList();

      _testOutputHelper.WriteJson(response);

      Assert.True(response.Any());
    }
  }
}
