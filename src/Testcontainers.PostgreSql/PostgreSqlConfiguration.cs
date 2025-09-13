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
    /// <param name="sslMode">The PostgreSql SSL mode.</param>
    /// <param name="rootCertFile">The path to the PostgreSql root certificate file.</param>
    /// <param name="clientCertFile">The path to the PostgreSql client certificate file.</param>
    /// <param name="clientKeyFile">The path to the PostgreSql client key file.</param>
    /// <param name="serverCertFile">The path to the PostgreSql server certificate file.</param>
    /// <param name="serverKeyFile">The path to the PostgreSql server key file.</param>
    /// <param name="caCertFile">The path to the PostgreSql CA certificate file.</param>
    public PostgreSqlConfiguration(
        string database = null,
        string username = null,
        string password = null,
        SslMode? sslMode = null,
        string rootCertFile = null,
        string clientCertFile = null,
        string clientKeyFile = null,
        string serverCertFile = null,
        string serverKeyFile = null,
        string caCertFile = null)
    {
        Database = database;
        Username = username;
        Password = password;
        SslMode = sslMode;
        RootCertFile = rootCertFile;
        ClientCertFile = clientCertFile;
        ClientKeyFile = clientKeyFile;
        ServerCertFile = serverCertFile;
        ServerKeyFile = serverKeyFile;
        CaCertFile = caCertFile;
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
        SslMode = BuildConfiguration.Combine(oldValue.SslMode, newValue.SslMode);
        RootCertFile = BuildConfiguration.Combine(oldValue.RootCertFile, newValue.RootCertFile);
        ClientCertFile = BuildConfiguration.Combine(oldValue.ClientCertFile, newValue.ClientCertFile);
        ClientKeyFile = BuildConfiguration.Combine(oldValue.ClientKeyFile, newValue.ClientKeyFile);
        ServerCertFile = BuildConfiguration.Combine(oldValue.ServerCertFile, newValue.ServerCertFile);
        ServerKeyFile = BuildConfiguration.Combine(oldValue.ServerKeyFile, newValue.ServerKeyFile);
        CaCertFile = BuildConfiguration.Combine(oldValue.CaCertFile, newValue.CaCertFile);
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

    /// <summary>
    /// Gets the PostgreSql SSL mode.
    /// </summary>
    public SslMode? SslMode { get; }

    /// <summary>
    /// Gets the path to the PostgreSql root certificate file.
    /// </summary>
    public string RootCertFile { get; }

    /// <summary>
    /// Gets the path to the PostgreSql client certificate file.
    /// </summary>
    public string ClientCertFile { get; }

    /// <summary>
    /// Gets the path to the PostgreSql client key file.
    /// </summary>
    public string ClientKeyFile { get; }

    /// <summary>
    /// Gets the path to the PostgreSql server certificate file.
    /// </summary>
    public string ServerCertFile { get; }

    /// <summary>
    /// Gets the path to the PostgreSql server key file.
    /// </summary>
    public string ServerKeyFile { get; }

    /// <summary>
    /// Gets the path to the PostgreSql CA certificate file.
    /// </summary>
    public string CaCertFile { get; }
}