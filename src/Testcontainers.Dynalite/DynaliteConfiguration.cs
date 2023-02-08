namespace Testcontainers.Dynalite;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DynaliteConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteConfiguration" /> class.
    /// </summary>
    public DynaliteConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynaliteConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynaliteConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DynaliteConfiguration(DynaliteConfiguration resourceConfiguration)
        : this(new DynaliteConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DynaliteConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public DynaliteConfiguration(DynaliteConfiguration oldValue, DynaliteConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
