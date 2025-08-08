namespace Testcontainers.Valkey;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ValkeyConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyConfiguration" /> class.
    /// </summary>
    public ValkeyConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ValkeyConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ValkeyConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ValkeyConfiguration(ValkeyConfiguration resourceConfiguration)
        : this(new ValkeyConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValkeyConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ValkeyConfiguration(ValkeyConfiguration oldValue, ValkeyConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}