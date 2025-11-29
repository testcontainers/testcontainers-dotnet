namespace Testcontainers.KurrentDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class KurrentDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbConfiguration" /> class.
    /// </summary>
    public KurrentDbConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KurrentDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KurrentDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KurrentDbConfiguration(KurrentDbConfiguration resourceConfiguration)
        : this(new KurrentDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KurrentDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public KurrentDbConfiguration(KurrentDbConfiguration oldValue, KurrentDbConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}