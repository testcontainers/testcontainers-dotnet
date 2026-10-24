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
    /// <param name="initScriptFilePaths">The host file paths of the init scripts.</param>
    /// <param name="initScriptContents">The file names and contents of the init scripts.</param>
    public MongoDbAtlasLocalConfiguration(
        string username = null,
        string password = null,
        IEnumerable<string> initScriptFilePaths = null,
        IEnumerable<KeyValuePair<string, string>> initScriptContents = null)
    {
        Username = username;
        Password = password;
        InitScriptFilePaths = initScriptFilePaths;
        InitScriptContents = initScriptContents;
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
        InitScriptFilePaths = BuildConfiguration.Combine(oldValue.InitScriptFilePaths, newValue.InitScriptFilePaths);
        InitScriptContents = BuildConfiguration.Combine(oldValue.InitScriptContents, newValue.InitScriptContents);
    }

    /// <summary>
    /// Gets the MongoDb Atlas Local username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the MongoDb Atlas Local password.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets the host file paths of the init scripts.
    /// </summary>
    public IEnumerable<string> InitScriptFilePaths { get; }

    /// <summary>
    /// Gets the file names and contents of the init scripts.
    /// </summary>
    public IEnumerable<KeyValuePair<string, string>> InitScriptContents { get; }
}