namespace Testcontainers.ServiceBus;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ServiceBusConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusConfiguration" /> class.
    /// </summary>
    /// <param name="databaseContainer">The database container.</param>
    public ServiceBusConfiguration(IDatabaseContainer databaseContainer = null)
    {
        DatabaseContainer = databaseContainer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ServiceBusConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ServiceBusConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ServiceBusConfiguration(ServiceBusConfiguration resourceConfiguration)
        : this(new ServiceBusConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceBusConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ServiceBusConfiguration(ServiceBusConfiguration oldValue, ServiceBusConfiguration newValue)
        : base(oldValue, newValue)
    {
        DatabaseContainer = BuildConfiguration.Combine(oldValue.DatabaseContainer, newValue.DatabaseContainer);
    }

    /// <summary>
    /// Gets the database container.
    /// </summary>
    public IDatabaseContainer DatabaseContainer { get; }
}