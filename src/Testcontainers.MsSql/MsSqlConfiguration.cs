namespace Testcontainers.MsSql;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class MsSqlConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="database">The MsSql database.</param>
    /// <param name="username">The MsSql username.</param>
    /// <param name="password">The MsSql password.</param>
    public MsSqlConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MsSqlConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MsSqlConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public MsSqlConfiguration(MsSqlConfiguration resourceConfiguration)
        : this(new MsSqlConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public MsSqlConfiguration(MsSqlConfiguration oldValue, MsSqlConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the MsSql database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the MsSql username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the MsSql password.
    /// </summary>
    public string Password { get; }
}