namespace Testcontainers.Ollama
{
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
            ImageName = OllamaConfiguration.ImageName;
        }
    
        /// <summary>
        /// Starts the Ollama container.
        /// </summary>
        public async Task StartOllamaAsync()
        {
            if (State!= TestcontainersStates.Created && State != TestcontainersStates.Running)                {
                throw new InvalidOperationException("Cannot start a container that has not been created.");
            }
            Task.WaitAll(ExecAsync(new List<string>()
            {
                "ollama", "run", ModelName,
            }));
            
            await Task.CompletedTask;
        }
    
        /// <summary>
        /// Gets the base URL of the Ollama API.
        /// </summary>
        /// <returns>The base URL of the Ollama API.</returns>
        /// <example>http://localhost:5000/api</example>
        public string GetBaseUrl() => $"http://{Hostname}:{GetMappedPublicPort(OllamaConfiguration.DefaultPort)}/api";
    
        /// <summary>
        /// Gets the name of the Docker image to use.
        /// </summary>
        public string ImageName { get; }
    
        /// <summary>
        /// Gets the name of the model to run.
        /// </summary>
        public string ModelName { get; }
    }
}