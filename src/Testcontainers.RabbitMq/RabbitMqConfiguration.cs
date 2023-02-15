namespace Testcontainers.RabbitMq;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class RabbitMqConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="config">The RabbitMq config.</param>
    public RabbitMqConfiguration(object config = null)
    {
        // // Sets the custom builder methods property values.
        // Config = config;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RabbitMqConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RabbitMqConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RabbitMqConfiguration(RabbitMqConfiguration resourceConfiguration)
        : this(new RabbitMqConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RabbitMqConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public RabbitMqConfiguration(RabbitMqConfiguration oldValue, RabbitMqConfiguration newValue)
        : base(oldValue, newValue)
    {
        // // Create an updated immutable copy of the module configuration.
        // Config = BuildConfiguration.Combine(oldValue.Config, newValue.Config);
    }

    // /// <summary>
    // /// Gets the RabbitMq config.
    // /// </summary>
    // public object Config { get; }
}