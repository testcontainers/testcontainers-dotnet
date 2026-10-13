namespace Testcontainers.MongoDbAtlasLocal;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MongoDbAtlasLocalConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalConfiguration" /> class.
    /// </summary>
    /// <param name="username">The MongoDb Atlas Local username.</param>
    /// <param name="password">The MongoDb Atlas Local password.</param>
    public MongoDbAtlasLocalConfiguration(
        string username = null,
        string password = null)
    {
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MongoDbAtlasLocalConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MongoDbAtlasLocalConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MongoDbAtlasLocalConfiguration(MongoDbAtlasLocalConfiguration resourceConfiguration)
        : this(new MongoDbAtlasLocalConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MongoDbAtlasLocalConfiguration(MongoDbAtlasLocalConfiguration oldValue, MongoDbAtlasLocalConfiguration newValue)
        : base(oldValue, newValue)
    {
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the MongoDb Atlas Local username.
    /// </summary>
    /// <remarks>
    /// If <c>null</c> or empty, the deployment does not enable authentication.
    /// </remarks>
    public string Username { get; }

    /// <summary>
    /// Gets the MongoDb Atlas Local password.
    /// </summary>
    /// <remarks>
    /// If <c>null</c> or empty, the deployment does not enable authentication.
    /// </remarks>
    public string Password { get; }
}