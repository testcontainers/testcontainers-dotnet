using System.Threading;

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
            Configuration = configuration;
        }

        public OllamaConfiguration Configuration { get; private set; }

        public Task Run(CancellationToken ct = default)
        {
            return Run(Configuration.ModelName, ct);
        }
    
        /// <summary>
        /// Starts the Ollama container.
        /// </summary>
        public Task Run(string modelName, CancellationToken ct = default)
        {
            ModelName = modelName;
            if (State!= TestcontainersStates.Created && State != TestcontainersStates.Running) {
                ThrowIfResourceNotFound();
            }

            return ExecAsync(new List<string>() {
                "ollama", "run", ModelName,
            }, ct);
        }
    
        /// <summary>
        /// Gets the base URL of the Ollama API.
        /// </summary>
        /// <returns>The base URL of the Ollama API.</returns>
        /// <example>http://localhost:5000/api</example>
        public string GetBaseUrl() => $"http://{Hostname}:{GetMappedPublicPort(OllamaBuilder.DefaultPort)}/api";
    
        /// <summary>
        /// Gets the name of the Docker image to use.
        /// </summary>
        public string ImageName { get; }
    
        /// <summary>
        /// Gets the name of the model to run.
        /// </summary>
        public string ModelName { get; private set; }
    }
}