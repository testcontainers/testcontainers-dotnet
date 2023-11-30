namespace Testcontainers.ArangoDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ArangoDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbConfiguration" /> class.
    /// </summary>
    /// <param name="password">The ArangoDb password.</param>
    public ArangoDbConfiguration(
        string password = null)
    {
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ArangoDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ArangoDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ArangoDbConfiguration(ArangoDbConfiguration resourceConfiguration)
        : this(new ArangoDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ArangoDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ArangoDbConfiguration(ArangoDbConfiguration oldValue, ArangoDbConfiguration newValue)
        : base(oldValue, newValue)
    {
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the ArangoDb password.
    /// </summary>
    public string Password { get; }
}