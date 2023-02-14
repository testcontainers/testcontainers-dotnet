namespace Testcontainers.Neo4j;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class Neo4jConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jConfiguration" /> class.
    /// </summary>
    public Neo4jConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public Neo4jConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public Neo4jConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public Neo4jConfiguration(Neo4jConfiguration resourceConfiguration)
        : this(new Neo4jConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Neo4jConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public Neo4jConfiguration(Neo4jConfiguration oldValue, Neo4jConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}