namespace Testcontainers.Ollama;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OllamaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public OllamaContainer(OllamaConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }
}