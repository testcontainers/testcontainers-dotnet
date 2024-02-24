namespace Testcontainers.PostgreSql;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class PostgreSqlBuilder : ContainerBuilder<PostgreSqlBuilder, PostgreSqlContainer, PostgreSqlConfiguration>
{
    public const string PostgreSqlImage = "postgres:15.1";

    public const ushort PostgreSqlPort = 5432;

    public const string DefaultDatabase = "postgres";

    public const string DefaultUsername = "postgres";

    public const string DefaultPassword = "postgres";

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

    /// <inheritdoc />
    public override PostgreSqlContainer Build()
    {
        Validate();

        // By default, the base builder waits until the container is running. However, for PostgreSql, a more advanced waiting strategy is necessary that requires access to the configured database and username.
        // If the user does not provide a custom waiting strategy, append the default PostgreSql waiting strategy.
        var postgreSqlBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new PostgreSqlContainer(postgreSqlBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
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
            _command = new List<string> { "pg_isready", "--host", "localhost", "--dbname", configuration.Database, "--username", configuration.Username };
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
                throw new NotSupportedException($"The '{container.Image.FullName}' image does not contain: pg_isready. Please use 'postgres:9.3' onwards.");
            }

            return 0L.Equals(execResult.ExitCode);
        }
    }
}