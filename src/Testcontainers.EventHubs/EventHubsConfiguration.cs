using Testcontainers.Azurite;

namespace Testcontainers.EventHubs;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class EventHubsConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="azuriteContainer">The Azurite docker container.</param>
    /// <param name="configurationBuilder">The configuration builder.</param>
    public EventHubsConfiguration(AzuriteContainer azuriteContainer = null,
        ConfigurationBuilder configurationBuilder = null)
    {
        ConfigurationBuilder = configurationBuilder;
        AzuriteContainer = azuriteContainer;
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
        ConfigurationBuilder = BuildConfiguration.Combine(oldValue.ConfigurationBuilder, newValue.ConfigurationBuilder);
        AzuriteContainer = BuildConfiguration.Combine(oldValue.AzuriteContainer, newValue.AzuriteContainer);
    }
    
    /// <summary>
    /// Gets the configuration builder
    /// </summary>
    public ConfigurationBuilder ConfigurationBuilder { get; }

    /// <summary>
    /// Gets the Azurite docker container details
    /// </summary>
    public AzuriteContainer AzuriteContainer { get; set; }
}