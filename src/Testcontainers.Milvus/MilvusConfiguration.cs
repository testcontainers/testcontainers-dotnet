namespace Testcontainers.Milvus;

public sealed class MilvusConfiguration : ContainerConfiguration
{

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusConfiguration" /> class.
    /// </summary>
    public MilvusConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MilvusConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MilvusConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MilvusConfiguration(MilvusConfiguration resourceConfiguration)
        : this(new MilvusConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MilvusConfiguration(MilvusConfiguration oldValue, MilvusConfiguration newValue)
        : base(oldValue, newValue)
    {
    }

    /// <summary>
    ///     An optional endpoint for an external etcd service. If <c>null</c>, uses an embedded etcd service.
    /// </summary>
    public string? EtcdEndpoint { get; }
}
