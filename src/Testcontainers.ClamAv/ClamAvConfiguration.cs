namespace Testcontainers.ClamAv;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ClamAvConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClamAvConfiguration" /> class.
    /// </summary>
    public ClamAvConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClamAvConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ClamAvConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClamAvConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ClamAvConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClamAvConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ClamAvConfiguration(ClamAvConfiguration resourceConfiguration)
        : this(new ClamAvConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClamAvConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ClamAvConfiguration(ClamAvConfiguration oldValue, ClamAvConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
