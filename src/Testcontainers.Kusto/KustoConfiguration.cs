namespace Testcontainers.Kusto;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class KustoConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KustoConfiguration" /> class.
    /// </summary>
    public KustoConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KustoConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KustoConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public KustoConfiguration(KustoConfiguration resourceConfiguration)
        : this(new KustoConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KustoConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public KustoConfiguration(KustoConfiguration oldValue, KustoConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}