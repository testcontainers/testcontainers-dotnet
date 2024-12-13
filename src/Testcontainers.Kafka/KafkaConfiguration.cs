using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Testcontainers.Kafka;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class KafkaConfiguration : ContainerConfiguration
{
    public IEnumerable<string> AdvertisedListeners { get; }
    public IEnumerable<string> Listeners { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="KafkaConfiguration" /> class.
    /// </summary>
    public KafkaConfiguration(IEnumerable<string> listeners = null, IEnumerable<string> advertisedListeners = null)
    {
        if ( listeners != null)
        {
            this.Listeners = [..listeners];
        }
        if (advertisedListeners != null)
        {
            this.AdvertisedListeners = [..advertisedListeners];
        }
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
        this.Listeners = BuildConfiguration.Combine<IEnumerable<string>>(oldValue.Listeners, newValue.Listeners);
        this.AdvertisedListeners = BuildConfiguration.Combine<IEnumerable<string>>(oldValue.AdvertisedListeners, newValue.AdvertisedListeners);
    }
}