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
    public KafkaConfiguration(
        IEnumerable<string> listeners = null,
        IEnumerable<string> advertisedListeners = null)
    {
        Listeners = listeners;
        AdvertisedListeners = advertisedListeners;
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
    }

    /// <summary>
    /// Gets a list of listeners.
    /// </summary>
    public IEnumerable<string> Listeners { get; }

    /// <summary>
    /// Gets a list of advertised listeners.
    /// </summary>
    public IEnumerable<string> AdvertisedListeners { get; }
}