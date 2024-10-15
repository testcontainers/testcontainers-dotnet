namespace Testcontainers.Seq;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class SeqConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SeqConfiguration" /> class.
    /// </summary>
    /// <param name="config">The Orion.TestContainers.Seq config.</param>
    public SeqConfiguration(object config = null)
    {
        // Sets the custom builder methods property values.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SeqConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SeqConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SeqConfiguration(SeqConfiguration resourceConfiguration)
        : this(new SeqConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SeqConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public SeqConfiguration(SeqConfiguration oldValue, SeqConfiguration newValue)
        : base(oldValue, newValue)
    {
        // Create an updated immutable copy of the module configuration.
    }
}