namespace Testcontainers.EventHubs;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class EventHubsConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="azuriteContainer">The Azurite container.</param>
    /// <param name="configurationBuilder">The Azure Event Hubs Emulator configuration.</param>
    public EventHubsConfiguration(AzuriteContainer azuriteContainer = null,
        ConfigurationBuilder configurationBuilder = null)
    {
        AzuriteContainer = azuriteContainer;
        ConfigurationBuilder = configurationBuilder;
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
        ConfigurationBuilder = BuildConfiguration.Combine(oldValue.ConfigurationBuilder, newValue.ConfigurationBuilder);
    }

    /// <summary>
    /// Gets the Azurite container.
    /// </summary>
    public AzuriteContainer AzuriteContainer { get; }

    /// <summary>
    /// Gets the Azure Event Hubs Emulator configuration.
    /// </summary>
    public ConfigurationBuilder ConfigurationBuilder { get; }
}