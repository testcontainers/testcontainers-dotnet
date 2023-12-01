namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AzuriteConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="inMemoryPersistence">Indicates whether in-memory persistence is enabled.</param>
    /// <param name="extentMemoryLimit">Optional memory-limit when using in-memory persistence.</param>
    public AzuriteConfiguration(bool inMemoryPersistence = false, int? extentMemoryLimit = null)
    {
        InMemoryPersistence = inMemoryPersistence;
        ExtentMemoryLimit = extentMemoryLimit;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzuriteConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzuriteConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public AzuriteConfiguration(AzuriteConfiguration resourceConfiguration)
        : this(new AzuriteConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public AzuriteConfiguration(AzuriteConfiguration oldValue, AzuriteConfiguration newValue)
        : base(oldValue, newValue)
    {
    }

    /// <summary>
    /// Gets a value indicating whether in-memory persistence is enabled.
    /// </summary>
    public bool InMemoryPersistence { get; }

    /// <summary>
    /// Gets a
    /// </summary>
    public int? ExtentMemoryLimit { get; }
}