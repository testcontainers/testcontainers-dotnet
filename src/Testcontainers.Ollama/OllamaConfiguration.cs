namespace Testcontainers.Ollama;

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
    public OllamaConfiguration(string modelName = null, string schema = null, string hostName = null, int? port = null)
    {
        ModelName = modelName;
        Schema = schema;
        HostName = hostName;
        Port = port ?? Port;
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
        Schema = BuildConfiguration.Combine(oldValue.Schema, newValue.Schema);
        HostName = BuildConfiguration.Combine(oldValue.HostName, newValue.HostName);
        Port = BuildConfiguration.Combine(oldValue.Port, newValue.Port);
    }
    
    public string ModelName { get; set; } = OllamaModels.Llama2;
    public string Schema { get; set; } = "http";
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 11434;
    
    /// <summary>
    /// Gets the name of the Docker image to use.
    /// </summary>
    public string ImageName { get; } = "ollama/ollama:latest";
}