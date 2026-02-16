namespace Testcontainers.Temporalio;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class TemporalConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    public TemporalConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TemporalConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TemporalConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TemporalConfiguration(TemporalConfiguration resourceConfiguration)
        : this(new TemporalConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TemporalConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public TemporalConfiguration(TemporalConfiguration oldValue, TemporalConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}