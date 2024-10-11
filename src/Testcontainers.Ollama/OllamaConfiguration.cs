namespace Testcontainers.Ollama
{
    /// <inheritdoc cref="ContainerConfiguration" />
    [PublicAPI]
    public sealed class OllamaConfiguration : ContainerConfiguration
    {
        /// <summary>
        /// The OllamaConfiguration class represents the configuration for an Ollama container.
        /// </summary>
        public OllamaConfiguration(string modelName = null, string volumePath = null, string volumeName = null, int? port = null)
        {
            ModelName = modelName ?? string.Empty;
            VolumePath = volumePath ?? OllamaBuilder.DefaultVolumePath;
            VolumeName = volumeName ?? OllamaBuilder.DefaultVolumeName;
            Port = port ?? OllamaBuilder.DefaultPort;
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
            VolumePath = BuildConfiguration.Combine(oldValue.VolumePath, newValue.VolumePath);
            VolumeName = BuildConfiguration.Combine(oldValue.VolumeName, newValue.VolumeName);
            Port = BuildConfiguration.Combine(oldValue.Port, newValue.Port);
        }

        /// <summary>
        /// Represents the configuration for the Ollama container.
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// The OllamaConfiguration class represents the configuration for an Ollama container.
        /// </summary>
        public string VolumePath { get; set; }

        /// <summary>
        /// Gets or sets the name of the volume associated with the Ollama container.
        /// </summary>
        public string VolumeName { get; set; }

        /// <summary>
        /// The <see cref="Port"/> class represents the configuration for an Ollama container port.
        /// </summary>
        public int Port { get; set; }
    }
}