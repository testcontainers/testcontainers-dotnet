namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class CosmosDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    public CosmosDbConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CosmosDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CosmosDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CosmosDbConfiguration(CosmosDbConfiguration resourceConfiguration)
        : this(new CosmosDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public CosmosDbConfiguration(CosmosDbConfiguration oldValue, CosmosDbConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}