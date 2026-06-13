namespace Testcontainers.Ollama;

/// <summary>
/// Provides the Ollama connection string.
/// </summary>
internal sealed class OllamaConnectionStringProvider : ContainerConnectionStringProvider<OllamaContainer, OllamaConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}