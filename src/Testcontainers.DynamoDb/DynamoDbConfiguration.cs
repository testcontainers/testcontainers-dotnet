namespace Testcontainers.DynamoDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DynamoDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbConfiguration" /> class.
    /// </summary>
    public DynamoDbConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynamoDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynamoDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynamoDbConfiguration(DynamoDbConfiguration resourceConfiguration)
        : this(new DynamoDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public DynamoDbConfiguration(DynamoDbConfiguration oldValue, DynamoDbConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}