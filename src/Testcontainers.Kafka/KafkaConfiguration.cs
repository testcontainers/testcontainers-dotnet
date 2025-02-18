namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class KafkaConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaConfiguration" /> class.
    /// </summary>
    /// <param name="listeners">A list of listeners.</param>
    /// <param name="advertisedListeners">A list of advertised listeners.</param>
    /// <param name="imageVendor">Kafka image vendor.</param>
    /// <param name="consensusProtocol">A consensus protocol to use.</param>
    /// <param name="externalZookeeperConnectionString">A connection string to an external Zookeeper.</param>
    public KafkaConfiguration(
        IEnumerable<string>? listeners = null,
        IEnumerable<string>? advertisedListeners = null,
        KafkaImageVendor? imageVendor = null,
        KafkaConsensusProtocol? consensusProtocol = null,
        string? externalZookeeperConnectionString = null)
    {
        Listeners = listeners;
        AdvertisedListeners = advertisedListeners;
        ImageVendor = imageVendor;
        ConsensusProtocol = consensusProtocol;
        ExternalZookeeperConnectionString = externalZookeeperConnectionString;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KafkaConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KafkaConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KafkaConfiguration(KafkaConfiguration resourceConfiguration)
        : this(new KafkaConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public KafkaConfiguration(KafkaConfiguration oldValue, KafkaConfiguration newValue)
        : base(oldValue, newValue)
    {
        Listeners = BuildConfiguration.Combine(oldValue.Listeners, newValue.Listeners);
        AdvertisedListeners = BuildConfiguration.Combine(oldValue.AdvertisedListeners, newValue.AdvertisedListeners);
        ImageVendor = BuildConfiguration.Combine(oldValue.ImageVendor, newValue.ImageVendor);
        ConsensusProtocol = BuildConfiguration.Combine(oldValue.ConsensusProtocol, newValue.ConsensusProtocol);
        ExternalZookeeperConnectionString = BuildConfiguration.Combine(oldValue.ExternalZookeeperConnectionString, newValue.ExternalZookeeperConnectionString);
    }

    /// <summary>
    /// Gets a list of listeners.
    /// </summary>
    public IEnumerable<string>? Listeners { get; }

    /// <summary>
    /// Gets a list of advertised listeners.
    /// </summary>
    public IEnumerable<string>? AdvertisedListeners { get; }

    /// <summary>
    /// Gets the Kafka image vendor.
    /// </summary>
    public KafkaImageVendor? ImageVendor { get; }

    /// <summary>
    /// Gets consensus protocol to use in Kafka.
    /// </summary>
    public KafkaConsensusProtocol? ConsensusProtocol { get; }

    /// <summary>
    /// Gets the external Zookeeper connection string.
    /// </summary>
    public string? ExternalZookeeperConnectionString { get; }
}