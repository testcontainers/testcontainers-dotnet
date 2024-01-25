namespace Testcontainers.Ollama;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class Testcontainers.OllamaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public Testcontainers.OllamaContainer(Testcontainers.OllamaConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}