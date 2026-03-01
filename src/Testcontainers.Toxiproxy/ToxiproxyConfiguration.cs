namespace Testcontainers.Toxiproxy;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ToxiproxyConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyConfiguration" /> class.
    /// </summary>
    public ToxiproxyConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ToxiproxyConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ToxiproxyConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ToxiproxyConfiguration(ToxiproxyConfiguration resourceConfiguration)
        : this(new ToxiproxyConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToxiproxyConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ToxiproxyConfiguration(ToxiproxyConfiguration oldValue, ToxiproxyConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}