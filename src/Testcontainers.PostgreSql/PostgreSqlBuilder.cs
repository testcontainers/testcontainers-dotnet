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
        return new PostgreSqlContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
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
            .WithCommand("-c", "synchronous_commit=off")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"));
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
}