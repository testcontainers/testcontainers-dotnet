namespace Testcontainers.RavenDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class RavenDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbConfiguration" /> class.
    /// </summary>
    public RavenDbConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RavenDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RavenDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public RavenDbConfiguration(RavenDbConfiguration resourceConfiguration)
        : this(new RavenDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RavenDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public RavenDbConfiguration(RavenDbConfiguration oldValue, RavenDbConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}