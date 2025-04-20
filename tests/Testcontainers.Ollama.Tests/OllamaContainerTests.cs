namespace Testcontainers.Ollama;

public sealed class OllamaContainerTest : IAsyncLifetime
{
    private readonly OllamaContainer _ollamaContainer = new OllamaBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _ollamaContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _ollamaContainer.DisposeAsync();
    }

    [Fact]
    public async Task GenerateEmbeddingsReturnsEmbeddings()
    {
        // Given
        const string model = "all-minilm:22m";

        using var ollamaClient = new OllamaApiClient(_ollamaContainer.GetBaseAddress());

        var embedRequest = new EmbedRequest();
        embedRequest.Model = model;
        embedRequest.Input = new List<string> { "Hello, World!" };

        // When
        await foreach (var _ in ollamaClient.PullModelAsync(model, TestContext.Current.CancellationToken)
            .ConfigureAwait(true));

        var embedResponse = await ollamaClient.EmbedAsync(embedRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.NotNull(embedResponse);
        Assert.NotEmpty(embedResponse.Embeddings);
    }
}