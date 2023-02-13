namespace Testcontainers.LocalStack;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class LocalStackConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    public LocalStackConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LocalStackConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LocalStackConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public LocalStackConfiguration(LocalStackConfiguration resourceConfiguration)
        : this(new LocalStackConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalStackConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public LocalStackConfiguration(LocalStackConfiguration oldValue, LocalStackConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}