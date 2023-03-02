namespace Testcontainers.SqlEdge;

/// <inheritdoc cref="ContainerConfiguration" />
[PublicAPI]
public sealed class SqlEdgeConfiguration : ContainerConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeConfiguration" /> class.
    /// </summary>
    /// <param name="database">The SqlEdge database.</param>
    /// <param name="username">The SqlEdge username.</param>
    /// <param name="password">The SqlEdge password.</param>
    public SqlEdgeConfiguration(
        string database = null,
        string username = null,
        string password = null)
    {
        Database = database;
        Username = username;
        Password = password;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SqlEdgeConfiguration(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SqlEdgeConfiguration(IContainerConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeConfiguration" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public SqlEdgeConfiguration(SqlEdgeConfiguration resourceConfiguration)
        : this(new SqlEdgeConfiguration(), resourceConfiguration)
    {
        // Passes the configuration upwards to the base implementations to create an updated immutable copy.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeConfiguration" /> class.
    /// </summary>
    /// <param name="oldValue">The old Docker resource configuration.</param>
    /// <param name="newValue">The new Docker resource configuration.</param>
    public SqlEdgeConfiguration(SqlEdgeConfiguration oldValue, SqlEdgeConfiguration newValue)
        : base(oldValue, newValue)
    {
        Database = BuildConfiguration.Combine(oldValue.Database, newValue.Database);
        Username = BuildConfiguration.Combine(oldValue.Username, newValue.Username);
        Password = BuildConfiguration.Combine(oldValue.Password, newValue.Password);
    }

    /// <summary>
    /// Gets the SqlEdge database.
    /// </summary>
    public string Database { get; }

    /// <summary>
    /// Gets the SqlEdge username.
    /// </summary>
    public string Username { get; }

    /// <summary>
    /// Gets the SqlEdge password.
    /// </summary>
    public string Password { get; }
}