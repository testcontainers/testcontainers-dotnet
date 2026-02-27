namespace Testcontainers.Ollama;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class OllamaConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaConfiguration" /> class.
    /// </summary>
    public OllamaConfiguration()
    {
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
    }
}