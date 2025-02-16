namespace Testcontainers.EventHubs;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class EventHubsConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="azuriteContainer">The Azurite container.</param>
    /// <param name="serviceConfiguration">The service configuration.</param>
    public EventHubsConfiguration(AzuriteContainer azuriteContainer = null,
        EventHubsServiceConfiguration serviceConfiguration = null)
    {
        AzuriteContainer = azuriteContainer;
        ServiceConfiguration = serviceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventHubsConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventHubsConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public EventHubsConfiguration(EventHubsConfiguration resourceConfiguration)
        : this(new EventHubsConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public EventHubsConfiguration(EventHubsConfiguration oldValue, EventHubsConfiguration newValue)
        : base(oldValue, newValue)
    {
        AzuriteContainer = BuildConfiguration.Combine(oldValue.AzuriteContainer, newValue.AzuriteContainer);
        ServiceConfiguration = BuildConfiguration.Combine(oldValue.ServiceConfiguration, newValue.ServiceConfiguration);
    }

    /// <summary>
    /// Gets the Azurite container.
    /// </summary>
    public AzuriteContainer AzuriteContainer { get; }

    /// <summary>
    /// Gets the service configuration.
    /// </summary>
    public EventHubsServiceConfiguration ServiceConfiguration { get; }
}