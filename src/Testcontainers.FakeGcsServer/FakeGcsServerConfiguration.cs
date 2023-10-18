namespace Testcontainers.FakeGcsServer;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class FakeGcsServerConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerConfiguration" /> class.
    /// </summary>
    public FakeGcsServerConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FakeGcsServerConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FakeGcsServerConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FakeGcsServerConfiguration(FakeGcsServerConfiguration resourceConfiguration)
        : this(new FakeGcsServerConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeGcsServerConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public FakeGcsServerConfiguration(FakeGcsServerConfiguration oldValue, FakeGcsServerConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}