namespace Testcontainers.Ollama
{
    /// <inheritdoc cref="ContainerConfiguration" />
    [PublicAPI]
    public sealed class OllamaConfiguration : ContainerConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaConfiguration" /> class.
        /// </summary>
        /// <param name="modelName"></param>
        /// <param name="schema"></param>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        public OllamaConfiguration(string modelName = null, string schema = null, int? port = null)
        {
            ModelName = modelName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaConfiguration" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        public OllamaConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
            : base(resourceConfiguration)
        {
            // Passes the configuration upwards to the base implementations to create an updated immutable copy.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaConfiguration" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        public OllamaConfiguration(IContainerConfiguration resourceConfiguration)
            : base(resourceConfiguration)
        {
            // Passes the configuration upwards to the base implementations to create an updated immutable copy.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaConfiguration" /> class.
        /// </summary>
        /// <param name="resourceConfiguration">The Docker resource configuration.</param>
        public OllamaConfiguration(OllamaConfiguration resourceConfiguration)
            : this(new OllamaConfiguration(), resourceConfiguration)
        {
            // Passes the configuration upwards to the base implementations to create an updated immutable copy.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OllamaConfiguration" /> class.
        /// </summary>
        /// <param name="oldValue">The old Docker resource configuration.</param>
        /// <param name="newValue">The new Docker resource configuration.</param>
        public OllamaConfiguration(OllamaConfiguration oldValue, OllamaConfiguration newValue)
            : base(oldValue, newValue)
        {
            ModelName = BuildConfiguration.Combine(oldValue.ModelName, newValue.ModelName);
        }
    
        /// <summary>
        /// Name of the model to use.
        /// </summary>
        public string ModelName { get; set; } = OllamaModels.Llama2;
    
        /// <summary>
        /// Gets the default port of the Ollama API.
        /// </summary>
        public const int DefaultPort = 11434;
        
        /// <summary>
        /// Default image name.
        /// </summary>
        public const string ImageName = "ollama/ollama:latest";
    
        /// <summary>
        /// Default volume path.
        /// </summary>
        public const string DefaultVolumePath = "/root/.ollama";
    
        /// <summary>
        /// Default volume name.
        /// </summary>
        public const string DefaultVolumeName = "ollama-volume";
    }
}