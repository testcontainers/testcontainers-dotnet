namespace Testcontainers.Azurite;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class AzuriteConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AzuriteConfiguration" /> class.
    /// </summary>
    public AzuriteConfiguration()
    {
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
}