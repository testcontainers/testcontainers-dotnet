namespace Testcontainers.PostgreSql;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class
    PostgreSqlBuilder : ContainerBuilder<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>
{
    public const string PostgreSqlImage = "postgres:15.1";

    public const ushort PostgreSqlPort = 5432;

    public const string DefaultDatabase = "postgres";

    public const string DefaultUsername = "postgres";

    public const string DefaultPassword = "postgres";

    private const string DefaultCertificatesDirectory = "/var/lib/postgresql/certs";

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlBuilder" /> class.
    /// </summary>
    public PostgreSqlBuilder()
        : this(new PostgreSqlConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private PostgreSqlBuilder(PostgreSqlConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override PostgreSqlConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the PostgreSql database.
    /// </summary>
    /// <param name="database">The PostgreSql database.</param>
    /// <returns>A configured instance of <see cref="PostgreSqlBuilder" />.</returns>
    public PostgreSqlBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(database: database))
            .WithEnvironment("POSTGRES_DB", database);
    }

    /// <summary>
    /// Sets the PostgreSql username.
    /// </summary>
    /// <param name="username">The PostgreSql username.</param>
    /// <returns>A configured instance of <see cref="PostgreSqlBuilder" />.</returns>
    public PostgreSqlBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(username: username))
            .WithEnvironment("POSTGRES_USER", username);
    }

    /// <summary>
    /// Sets the PostgreSql password.
    /// </summary>
    /// <param name="password">The PostgreSql password.</param>
    /// <returns>A configured instance of <see cref="PostgreSqlBuilder" />.</returns>
    public PostgreSqlBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(password: password))
            .WithEnvironment("POSTGRES_PASSWORD", password);
    }

    /// <summary>
    /// Sets the PostgreSql SSL mode.
    /// </summary>
    /// <param name="sslMode">The PostgreSql SSL mode.</param>
    /// <returns>A configured instance of <see cref="PostgreSqlBuilder" />.</returns>
    public PostgreSqlBuilder WithSslMode(SslMode sslMode)
    {
        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(sslMode: sslMode))
            .WithEnvironment("PGSSLMODE", sslMode.ToString().ToLowerInvariant());
    }

    /// <summary>
    /// Sets the PostgreSql root certificate file.
    /// </summary>
    /// <param name="rootCertFile">The path to the root certificate file.</param>
    /// <returns>A configured instance of <see cref="PostgreSqlBuilder" />.</returns>
    public PostgreSqlBuilder WithRootCertificate(string rootCertFile)
    {
        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(rootCertFile: rootCertFile))
            .WithBindMount(rootCertFile, Path.Combine(DefaultCertificatesDirectory, "root.crt"), AccessMode.ReadOnly)
            .WithEnvironment("PGSSLROOTCERT", Path.Combine(DefaultCertificatesDirectory, "root.crt"));
    }

    /// <summary>
    /// Sets the PostgreSql client certificate and key files.
    /// </summary>
    /// <param name="clientCertFile">The path to the client certificate file.</param>
    /// <param name="clientKeyFile">The path to the client key file.</param>
    /// <returns>A configured instance of <see cref="PostgreSqlBuilder" />.</returns>
    public PostgreSqlBuilder WithClientCertificate(string clientCertFile, string clientKeyFile)
    {
        return Merge(DockerResourceConfiguration,
                new PostgreSqlConfiguration(clientCertFile: clientCertFile, clientKeyFile: clientKeyFile))
            .WithBindMount(clientCertFile, Path.Combine(DefaultCertificatesDirectory, "postgresql.crt"),
                AccessMode.ReadOnly)
            .WithBindMount(clientKeyFile, Path.Combine(DefaultCertificatesDirectory, "postgresql.key"),
                AccessMode.ReadOnly)
            .WithEnvironment("PGSSLCERT", Path.Combine(DefaultCertificatesDirectory, "postgresql.crt"))
            .WithEnvironment("PGSSLKEY", Path.Combine(DefaultCertificatesDirectory, "postgresql.key"));
    }

    /// <summary>
    /// Configures the PostgreSQL server to run with SSL using the provided CA certificate, server certificate and private key.
    /// This enables server-side SSL configuration with client certificate authentication.
    /// </summary>
    /// <param name="caCertFile">The path to the CA certificate file.</param>
    /// <param name="serverCertFile">The path to the server certificate file.</param>
    /// <param name="serverKeyFile">The path to the server private key file.</param>
    /// <returns>A configured instance of <see cref="PostgreSqlBuilder" />.</returns>
    /// <remarks>
    /// This method configures PostgreSQL for server-side SSL with client certificate authentication.
    /// It requires a custom PostgreSQL configuration file that enables SSL and sets the appropriate
    /// certificate paths. The certificates are mounted into the container and PostgreSQL is configured
    /// to use them for SSL connections.
    /// </remarks>
    public PostgreSqlBuilder WithSSLSettings(string caCertFile, string serverCertFile, string serverKeyFile)
    {
        if (string.IsNullOrWhiteSpace(caCertFile))
        {
            throw new ArgumentException("CA certificate file path cannot be null or empty.", nameof(caCertFile));
        }

        if (string.IsNullOrWhiteSpace(serverCertFile))
        {
            throw new ArgumentException("Server certificate file path cannot be null or empty.",
                nameof(serverCertFile));
        }

        if (string.IsNullOrWhiteSpace(serverKeyFile))
        {
            throw new ArgumentException("Server key file path cannot be null or empty.", nameof(serverKeyFile));
        }

        const string sslConfigDir = "/tmp/testcontainers-dotnet/postgres";

        var wrapperEntrypoint = @"#!/bin/sh
set -e
SSL_DIR=/tmp/testcontainers-dotnet/postgres
# Fix ownership and permissions for SSL key/cert before Postgres init runs
if [ -f ""$SSL_DIR/server.key"" ]; then
  chown postgres:postgres ""$SSL_DIR/server.key"" || true
  chmod 600 ""$SSL_DIR/server.key"" || true
fi
if [ -f ""$SSL_DIR/server.crt"" ]; then
  chown postgres:postgres ""$SSL_DIR/server.crt"" || true
fi
if [ -f ""$SSL_DIR/ca_cert.pem"" ]; then
  chown postgres:postgres ""$SSL_DIR/ca_cert.pem"" || true
fi
exec /usr/local/bin/docker-entrypoint.sh ""$@""
";

        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(
                serverCertFile: serverCertFile,
                serverKeyFile: serverKeyFile,
                caCertFile: caCertFile))
            .WithResourceMapping(File.ReadAllBytes(caCertFile), $"{sslConfigDir}/ca_cert.pem", Unix.FileMode644)
            .WithResourceMapping(File.ReadAllBytes(serverCertFile), $"{sslConfigDir}/server.crt", Unix.FileMode644)
            .WithResourceMapping(File.ReadAllBytes(serverKeyFile), $"{sslConfigDir}/server.key", Unix.FileMode700)
            .WithResourceMapping(Encoding.UTF8.GetBytes(wrapperEntrypoint), "/usr/local/bin/docker-entrypoint-ssl.sh",
                Unix.FileMode755)
            .WithEntrypoint("/usr/local/bin/docker-entrypoint-ssl.sh")
            .WithCommand("-c", "ssl=on")
            .WithCommand("-c", $"ssl_ca_file={sslConfigDir}/ca_cert.pem")
            .WithCommand("-c", $"ssl_cert_file={sslConfigDir}/server.crt")
            .WithCommand("-c", $"ssl_key_file={sslConfigDir}/server.key");
    }

    /// <inheritdoc />
    public override PostgreSqlContainer Build()
    {
        Validate();

        // Ensure PostgreSQL is actually ready to accept connections over TCP, not just that the container is running.
        // Always append the pg_isready-based wait strategy by default so tests using the default fixture are stable.
        var postgreSqlBuilder =
            WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new PostgreSqlContainer(postgreSqlBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override PostgreSqlBuilder Init()
    {
        return base.Init()
            .WithImage(PostgreSqlImage)
            .WithPortBinding(PostgreSqlPort, true)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            // Disable durability: https://www.postgresql.org/docs/current/non-durability.html.
            .WithCommand("-c", "fsync=off")
            .WithCommand("-c", "full_page_writes=off")
            .WithCommand("-c", "synchronous_commit=off");
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override PostgreSqlBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PostgreSqlBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new PostgreSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override PostgreSqlBuilder Merge(PostgreSqlConfiguration oldValue, PostgreSqlConfiguration newValue)
    {
        return new PostgreSqlBuilder(new PostgreSqlConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly IList<string> _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(PostgreSqlConfiguration configuration)
        {
            // Explicitly specify the host to ensure readiness only after the initdb scripts have executed, and the server is listening on TCP/IP.
            _command = new List<string>
            {
                "pg_isready", "--host", "localhost", "--dbname", configuration.Database, "--username",
                configuration.Username
            };
        }

        /// <summary>
        /// Checks whether the database is ready and accepts connections or not.
        /// </summary>
        /// <remarks>
        /// The wait strategy uses <a href="https://www.postgresql.org/docs/current/app-pg-isready.html">pg_isready</a> to check the connection status of PostgreSql.
        /// </remarks>
        /// <param name="container">The starting container instance.</param>
        /// <returns>Task that completes and returns true when the database is ready and accepts connections, otherwise false.</returns>
        /// <exception cref="NotSupportedException">Thrown when the PostgreSql image does not contain <c>pg_isready</c>.</exception>
        public async Task<bool> UntilAsync(IContainer container)
        {
            var execResult = await container.ExecAsync(_command)
                .ConfigureAwait(false);

            if (execResult.Stderr.Contains("pg_isready was not found"))
            {
                throw new NotSupportedException(
                    $"The '{container.Image.FullName}' image does not contain: pg_isready. Please use 'postgres:9.3' onwards.");
            }

            return 0L.Equals(execResult.ExitCode);
        }
    }
}