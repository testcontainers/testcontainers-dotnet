namespace Testcontainers.CosmosDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class CosmosDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="partitionCount">The number of partitions to create</param>
    /// <param name="ipAddressOverride">The overridden IP address.</param>
    public CosmosDbConfiguration(
        int? partitionCount = null,
        string ipAddressOverride = null)
    {
        PartitionCount = partitionCount;
        IpAddressOverride = ipAddressOverride;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CosmosDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CosmosDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CosmosDbConfiguration(CosmosDbConfiguration resourceConfiguration)
        : this(new CosmosDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CosmosDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public CosmosDbConfiguration(CosmosDbConfiguration oldValue, CosmosDbConfiguration newValue)
        : base(oldValue, newValue)
    {
        PartitionCount = BuildConfiguration.Combine(oldValue.PartitionCount, newValue.PartitionCount);
        IpAddressOverride = BuildConfiguration.Combine(oldValue.IpAddressOverride, newValue.IpAddressOverride);
    }

    /// <summary>
    /// Gets the partition count
    /// </summary>
    public int? PartitionCount { get; }

    /// <summary>
    /// Gets the overridden IP address 
    /// </summary>
    public string IpAddressOverride { get; }
}