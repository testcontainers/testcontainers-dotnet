namespace Testcontainers.SpiceDB;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class SpiceDBConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBConfiguration" /> class.
    /// </summary>
    public SpiceDBConfiguration(string grpcPresharedKey = "mysecret", string datastoreEngine = "memory", bool? tslEnabled = false)
    {
        GrpcPresharedKey = grpcPresharedKey;
        DatastoreEngine = datastoreEngine;
        TslEnabled = tslEnabled.GetValueOrDefault(false);
    }

    public bool TslEnabled { get; set; }

    public string GrpcPresharedKey { get; set; }

    public string DatastoreEngine { get; set; }


    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SpiceDBConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SpiceDBConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SpiceDBConfiguration(SpiceDBConfiguration resourceConfiguration)
        : this(new SpiceDBConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpiceDBConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public SpiceDBConfiguration(SpiceDBConfiguration oldValue, SpiceDBConfiguration newValue)
        : base(oldValue, newValue)
    {
    }
}
