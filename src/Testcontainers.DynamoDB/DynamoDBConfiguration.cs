namespace Testcontainers.DynamoDB;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DynamoDBConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBConfiguration" /> class.
    /// </summary>
    public DynamoDBConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynamoDBConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynamoDBConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynamoDBConfiguration(DynamoDBConfiguration resourceConfiguration)
        : this(new DynamoDBConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamoDBConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public DynamoDBConfiguration(DynamoDBConfiguration oldValue, DynamoDBConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
