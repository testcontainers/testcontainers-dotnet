namespace Testcontainers.PubSub;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class PubSubConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubConfiguration" /> class.
    /// </summary>
    /// <param name="config">The PubSub config.</param>
    public PubSubConfiguration(object config = null)
    {
        // // Sets the custom builder methods property values.
        // Config = config;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PubSubConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PubSubConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PubSubConfiguration(PubSubConfiguration resourceConfiguration)
        : this(new PubSubConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PubSubConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public PubSubConfiguration(PubSubConfiguration oldValue, PubSubConfiguration newValue)
        : base(oldValue, newValue)
    {
        // // Create an updated immutable copy of the module configuration.
        // Config = BuildConfiguration.Combine(oldValue.Config, newValue.Config);
    }

    // /// <summary>
    // /// Gets the PubSub config.
    // /// </summary>
    // public object Config { get; }
}