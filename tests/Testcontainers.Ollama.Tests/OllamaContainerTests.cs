namespace Testcontainers.Ollama;

public sealed class OllamaContainerTest : IAsyncLifetime
{
    private readonly OllamaContainer _ollamaContainer = new OllamaBuilder().Build();

    public Task InitializeAsync()
    {
        return _ollamaContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _ollamaContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task TODO()
    {
    }
}