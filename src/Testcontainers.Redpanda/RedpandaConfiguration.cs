namespace Testcontainers.Redpanda;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class RedpandaConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaConfiguration" /> class.
    /// </summary>
    public RedpandaConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RedpandaConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RedpandaConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RedpandaConfiguration(RedpandaConfiguration resourceConfiguration)
        : this(new RedpandaConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RedpandaConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public RedpandaConfiguration(RedpandaConfiguration oldValue, RedpandaConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}