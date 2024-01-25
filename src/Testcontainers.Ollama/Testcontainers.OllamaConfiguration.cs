namespace Testcontainers.Ollama;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class Testcontainers.OllamaConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaConfiguration" /> class.
    /// </summary>
    /// <param name="config">The Testcontainers.Ollama config.</param>
    public Testcontainers.OllamaConfiguration(object config = null)
    {
        // // Sets the custom builder methods property values.
        // Config = config;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public Testcontainers.OllamaConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public Testcontainers.OllamaConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public Testcontainers.OllamaConfiguration(Testcontainers.OllamaConfiguration resourceConfiguration)
        : this(new Testcontainers.OllamaConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Testcontainers.OllamaConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public Testcontainers.OllamaConfiguration(Testcontainers.OllamaConfiguration oldValue, Testcontainers.OllamaConfiguration newValue)
        : base(oldValue, newValue)
    {
        // // Create an updated immutable copy of the module configuration.
        // Config = BuildConfiguration.Combine(oldValue.Config, newValue.Config);
    }

    // /// <summary>
    // /// Gets the Testcontainers.Ollama config.
    // /// </summary>
    // public object Config { get; }
}