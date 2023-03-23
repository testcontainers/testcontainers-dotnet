namespace Testcontainers.JanusGraph;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class JanusGraphConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphConfiguration" /> class.
    /// </summary>
    public JanusGraphConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public JanusGraphConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public JanusGraphConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public JanusGraphConfiguration(JanusGraphConfiguration resourceConfiguration)
        : this(new JanusGraphConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JanusGraphConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public JanusGraphConfiguration(JanusGraphConfiguration oldValue, JanusGraphConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}