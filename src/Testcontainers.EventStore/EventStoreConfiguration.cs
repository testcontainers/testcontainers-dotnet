namespace Testcontainers.EventStore;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class EventStoreConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConfiguration" /> class.
    /// </summary>
    public EventStoreConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventStoreConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventStoreConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventStoreConfiguration(EventStoreConfiguration resourceConfiguration)
        : this(new EventStoreConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventStoreConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public EventStoreConfiguration(EventStoreConfiguration oldValue, EventStoreConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}