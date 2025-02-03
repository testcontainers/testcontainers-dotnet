namespace Testcontainers.Weaviate;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class WeaviateConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateConfiguration" /> class.
    /// </summary>
    public WeaviateConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WeaviateConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WeaviateConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public WeaviateConfiguration(WeaviateConfiguration resourceConfiguration)
        : this(new WeaviateConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public WeaviateConfiguration(WeaviateConfiguration oldValue, WeaviateConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}