namespace Testcontainers.MariaDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MariaDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbConfiguration" /> class.
    /// </summary>
    /// <param name="database">The MariaDb database.</param>
    /// <param name="username">The MariaDb username.</param>
    /// <param name="password">The MariaDb password.</param>
    public MariaDbConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MariaDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MariaDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MariaDbConfiguration(MariaDbConfiguration resourceConfiguration)
        : this(new MariaDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MariaDbConfiguration(MariaDbConfiguration oldValue, MariaDbConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the MariaDb database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the MariaDb username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the MariaDb password.
    /// </summary>
    public string Password { get; }
}