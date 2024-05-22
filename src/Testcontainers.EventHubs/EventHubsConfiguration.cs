namespace Testcontainers.EventHubs;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class EventHubsConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventHubsConfiguration" /> class.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder.</param>
    /// <param name="azuriteBlobEndpoint">The Azurite blob endpoint.</param>
    /// <param name="azuriteTableEndpoint">The Azurite table endpoint.</param>
    public EventHubsConfiguration(
        ConfigurationBuilder configurationBuilder = null,
        string azuriteBlobEndpoint = null,
        string azuriteTableEndpoint = null)
    {
        ConfigurationBuilder = configurationBuilder;
        AzuriteBlobEndpoint = azuriteBlobEndpoint;
        AzuriteTableEndpoint = azuriteTableEndpoint;
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
        AzuriteBlobEndpoint = BuildConfiguration.Combine(oldValue.AzuriteBlobEndpoint, newValue.AzuriteBlobEndpoint);
        AzuriteTableEndpoint = BuildConfiguration.Combine(oldValue.AzuriteTableEndpoint, newValue.AzuriteTableEndpoint);
    }
    
    /// <summary>
    /// Gets the configuration builder
    /// </summary>
    public ConfigurationBuilder ConfigurationBuilder { get; }
    
    /// <summary>
    /// Gets the Azurite blob endpoint
    /// </summary>
    public string AzuriteBlobEndpoint { get; }
    
    /// <summary>
    /// Gets the Azurite table endpoint
    /// </summary>
    public string AzuriteTableEndpoint { get; }
}