namespace Testcontainers.Triton;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class TritonConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TritonConfiguration" /> class.
    /// </summary>
    public TritonConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TritonConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TritonConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TritonConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TritonConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TritonConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public TritonConfiguration(TritonConfiguration resourceConfiguration)
        : this(new TritonConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TritonConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public TritonConfiguration(TritonConfiguration oldValue, TritonConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
