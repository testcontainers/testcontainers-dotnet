namespace Testcontainers.Ollama;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OllamaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public OllamaContainer(OllamaConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
        ModelName = configuration.ModelName;
        Schema = configuration.Schema;
        HostName = configuration.HostName;
        Port = configuration.Port;
        ImageName = configuration.ImageName;
    }
    
    public string GetBaseUrl() => $"{Schema}://{HostName}:{Port}/api";

    public string Schema { get; }
    public string HostName { get; }
    public int Port { get; }
    
    /// <summary>
    /// Gets the name of the Docker image to use.
    /// </summary>
    public string ImageName { get; }
    
    /// <summary>
    /// Gets the name of the model to run.
    /// </summary>
    public string ModelName { get; }
}