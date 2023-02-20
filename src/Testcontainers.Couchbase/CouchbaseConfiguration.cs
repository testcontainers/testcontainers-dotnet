namespace Testcontainers.Couchbase;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class CouchbaseConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CouchbaseConfiguration" /> class.
    /// </summary>
    /// <param name="config">The Couchbase config.</param>
    public CouchbaseConfiguration(object config = null)
    {
        // // Sets the custom builder methods property values.
        // Config = config;
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
        // // Create an updated immutable copy of the module configuration.
        // Config = BuildConfiguration.Combine(oldValue.Config, newValue.Config);
    }

    // /// <summary>
    // /// Gets the Couchbase config.
    // /// </summary>
    // public object Config { get; }
}