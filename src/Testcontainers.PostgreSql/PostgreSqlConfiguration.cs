namespace Testcontainers.PostgreSql;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class PostgreSqlConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlConfiguration" /> class.
    /// </summary>
    /// <param name="database">The PostgreSql database.</param>
    /// <param name="username">The PostgreSql username.</param>
    /// <param name="password">The PostgreSql password.</param>
    public PostgreSqlConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PostgreSqlConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PostgreSqlConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public PostgreSqlConfiguration(PostgreSqlConfiguration resourceConfiguration)
        : this(new PostgreSqlConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public PostgreSqlConfiguration(PostgreSqlConfiguration oldValue, PostgreSqlConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the PostgreSql database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the PostgreSql username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the PostgreSql password.
    /// </summary>
    public string Password { get; }
}