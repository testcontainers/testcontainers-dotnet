namespace Testcontainers.Couchbase;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class CouchbaseConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseConfiguration" /> class.
    /// </summary>
    /// <param name="buckets">A list of Couchbase buckets.</param>
    public CouchbaseConfiguration(IEnumerable<CouchbaseBucket> buckets = null)
    {
        Buckets = buckets;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CouchbaseConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CouchbaseConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CouchbaseConfiguration(CouchbaseConfiguration resourceConfiguration)
        : this(new CouchbaseConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public CouchbaseConfiguration(CouchbaseConfiguration oldValue, CouchbaseConfiguration newValue)
        : base(oldValue, newValue)
    {
        Buckets = BuildConfiguration.Combine(oldValue.Buckets, newValue.Buckets);
    }

    /// <summary>
    /// Gets a list of Couchbase buckets.
    /// </summary>
    public IEnumerable<CouchbaseBucket> Buckets { get; } = Array.Empty<CouchbaseBucket>();
}