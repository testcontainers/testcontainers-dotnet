namespace Testcontainers.ClickHouse;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class ClickHouseConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseConfiguration" /> class.
    /// </summary>
    /// <param name="database">The ClickHouse database.</param>
    /// <param name="username">The ClickHouse username.</param>
    /// <param name="password">The ClickHouse password.</param>
    public ClickHouseConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ClickHouseConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ClickHouseConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public ClickHouseConfiguration(ClickHouseConfiguration resourceConfiguration)
        : this(new ClickHouseConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ClickHouseConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public ClickHouseConfiguration(ClickHouseConfiguration oldValue, ClickHouseConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the ClickHouse database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the ClickHouse username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the ClickHouse password.
    /// </summary>
    public string Password { get; }
}