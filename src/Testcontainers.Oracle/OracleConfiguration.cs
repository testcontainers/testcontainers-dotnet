namespace Testcontainers.Oracle;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class OracleConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OracleConfiguration" /> class.
    /// </summary>
    /// <param name="database">The Oracle database.</param>
    /// <param name="username">The Oracle username.</param>
    /// <param name="password">The Oracle password.</param>
    public OracleConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OracleConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OracleConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public OracleConfiguration(OracleConfiguration resourceConfiguration)
        : this(new OracleConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public OracleConfiguration(OracleConfiguration oldValue, OracleConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the Oracle database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the Oracle username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the Oracle password.
    /// </summary>
    public string Password { get; }
}