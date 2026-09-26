namespace Testcontainers.ZooKeeper;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ZooKeeperConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperConfiguration" /> class.
    /// </summary>
    public ZooKeeperConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ZooKeeperConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ZooKeeperConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ZooKeeperConfiguration(ZooKeeperConfiguration resourceConfiguration)
        : this(new ZooKeeperConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZooKeeperConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ZooKeeperConfiguration(ZooKeeperConfiguration oldValue, ZooKeeperConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
