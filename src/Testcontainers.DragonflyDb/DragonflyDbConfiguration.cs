namespace Testcontainers.DragonflyDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class DragonflyDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbConfiguration" /> class.
    /// </summary>
    public DragonflyDbConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DragonflyDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DragonflyDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public DragonflyDbConfiguration(DragonflyDbConfiguration resourceConfiguration)
        : this(new DragonflyDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DragonflyDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public DragonflyDbConfiguration(DragonflyDbConfiguration oldValue, DragonflyDbConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}