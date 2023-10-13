namespace Testcontainers.GCS;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class GCSConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GCSConfiguration" /> class.
    /// </summary>
    public GCSConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GCSConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GCSConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GCSConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GCSConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GCSConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public GCSConfiguration(GCSConfiguration resourceConfiguration)
        : this(new GCSConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GCSConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public GCSConfiguration(GCSConfiguration oldValue, GCSConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}