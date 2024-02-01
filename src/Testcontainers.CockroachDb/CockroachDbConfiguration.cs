namespace Testcontainers.CockroachDb;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class CockroachDbConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbConfiguration" /> class.
    /// </summary>
    /// <param name="database">The CockroachDb database.</param>
    /// <param name="username">The CockroachDb username.</param>
    /// <param name="password">The CockroachDb password.</param>
    public CockroachDbConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CockroachDbConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CockroachDbConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public CockroachDbConfiguration(CockroachDbConfiguration resourceConfiguration)
        : this(new CockroachDbConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CockroachDbConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public CockroachDbConfiguration(CockroachDbConfiguration oldValue, CockroachDbConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the CockroachDb database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the CockroachDb username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the CockroachDb password.
    /// </summary>
    public string Password { get; }
}