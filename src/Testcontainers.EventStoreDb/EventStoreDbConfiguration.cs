namespace Testcontainers.EventStoreDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class EventStoreDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbConfiguration" /> class.
    /// </summary>
    public EventStoreDbConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventStoreDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventStoreDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventStoreDbConfiguration(EventStoreDbConfiguration resourceConfiguration)
        : this(new EventStoreDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public EventStoreDbConfiguration(EventStoreDbConfiguration oldValue, EventStoreDbConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}