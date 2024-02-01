namespace Testcontainers.Ollama.Tests
{
  public class OllamaContainerTests : IAsyncLifetime
  {
    private OllamaContainer _ollamaContainer;

    public async Task InitializeAsync()
    {
      _ollamaContainer = new OllamaBuilder()
        .OllamaConfig(new OllamaConfiguration(OllamaModels.Llama2))
        .Build();
      await _ollamaContainer.StartAsync();
      await _ollamaContainer.Run();
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

      Assert.True(response.Any());
    }
  }
}
