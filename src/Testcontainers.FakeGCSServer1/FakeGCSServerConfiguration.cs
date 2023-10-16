namespace Testcontainers.FakeGCSServer1;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class FakeGCSServerConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerConfiguration" /> class.
    /// </summary>
    public FakeGCSServerConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FakeGCSServerConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FakeGCSServerConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FakeGCSServerConfiguration(FakeGCSServerConfiguration resourceConfiguration)
        : this(new FakeGCSServerConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGCSServerConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public FakeGCSServerConfiguration(FakeGCSServerConfiguration oldValue, FakeGCSServerConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}