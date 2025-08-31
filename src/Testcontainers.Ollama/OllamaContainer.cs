namespace Testcontainers.Ollama;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OllamaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public OllamaContainer(OllamaConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the Ollama base address.
    /// </summary>
    /// <returns>The Ollama base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(OllamaBuilder.OllamaPort)
        ).ToString();
    }
}
