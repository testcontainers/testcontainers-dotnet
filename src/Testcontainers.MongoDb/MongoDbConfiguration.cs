namespace Testcontainers.MongoDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MongoDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbConfiguration" /> class.
    /// </summary>
    /// <param name="username">The MongoDb username.</param>
    /// <param name="password">The MongoDb password.</param>
    /// <param name="replicaSetName">The replica set name.</param>
    public MongoDbConfiguration(
        string username = null,
        string password = null,
        string replicaSetName = null)
    {
        Username = username;
        Password = password;
        ReplicaSetName = replicaSetName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MongoDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MongoDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MongoDbConfiguration(MongoDbConfiguration resourceConfiguration)
        : this(new MongoDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MongoDbConfiguration(MongoDbConfiguration oldValue, MongoDbConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
        ReplicaSetName = BuildConfiguration.Combine(oldValue.ReplicaSetName, newValue.ReplicaSetName);
    }

    /// <summary>
    /// Gets the MongoDb username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the MongoDb password.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets the replica set name.
    /// </summary>
    /// <remarks>
    /// If specified, the container will be started as a single-node replica set.
    /// </remarks>
    public string ReplicaSetName { get; }
}