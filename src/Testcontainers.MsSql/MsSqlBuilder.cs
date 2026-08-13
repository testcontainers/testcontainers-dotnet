namespace Testcontainers.MsSql;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MsSqlBuilder : ContainerBuilder<MsSqlBuilder, MsSqlContainer, MsSqlConfiguration>
{
    [Obsolete("This constant is obsolete and will be removed in the future. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    public const string MsSqlImage = "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";

    public const ushort MsSqlPort = 1433;

    public const string DefaultDatabase = "master";

    public const string DefaultUsername = "sa";

    public const string DefaultPassword = "yourStrong(!)Password";

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    [Obsolete("This parameterless constructor is obsolete and will be removed. Use the constructor with the image parameter instead: https://github.com/testcontainers/testcontainers-dotnet/discussions/1470#discussioncomment-15185721.")]
    [ExcludeFromCodeCoverage]
    public MsSqlBuilder()
        : this(MsSqlImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://mcr.microsoft.com/en-us/artifact/mar/mssql/server/tags" />.
    /// </remarks>
    public MsSqlBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://mcr.microsoft.com/en-us/artifact/mar/mssql/server/tags" />.
    /// </remarks>
    public MsSqlBuilder(IImage image)
        : this(new MsSqlConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MsSqlBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MsSqlBuilder(MsSqlConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MsSqlConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the MsSql database.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the database. It is created
    /// as part of the startup callback, which runs before any wait strategy.
    /// </remarks>
    /// <param name="database">The MsSql database.</param>
    /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    public MsSqlBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(database: database))
            .WithEnvironment("SQLCMDDBNAME", database);
    }

    /// <summary>
    /// Sets the MsSql password.
    /// </summary>
    /// <param name="password">The MsSql password.</param>
    /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    public MsSqlBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(password: password))
            .WithEnvironment("MSSQL_SA_PASSWORD", password)
            .WithEnvironment("SQLCMDPASSWORD", password);
    }

    /// <inheritdoc />
    public override MsSqlContainer Build()
    {
        Validate();
        return new MsSqlContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Init()
    {
        return base.Init()
            .WithPortBinding(MsSqlPort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithConnectionStringProvider(new MsSqlConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil()))
            .WithStartupCallback(CreateDatabaseAsync);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Database, nameof(DockerResourceConfiguration.Database))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MsSqlBuilder Merge(MsSqlConfiguration oldValue, MsSqlConfiguration newValue)
    {
        return new MsSqlBuilder(new MsSqlConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Sets the MsSql username.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the username.
    /// </remarks>
    /// <param name="username">The MsSql username.</param>
    /// <returns>A configured instance of <see cref="MsSqlBuilder" />.</returns>
    private MsSqlBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new MsSqlConfiguration(username: username))
            .WithEnvironment("SQLCMDUSER", username);
    }

    /// <summary>
    /// Creates the configured MsSql database.
    /// </summary>
    /// <remarks>
    /// Creating the default database is skipped.
    /// </remarks>
    /// <param name="container">The container instance.</param>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the database has been created.</returns>
    private static async Task CreateDatabaseAsync(MsSqlContainer container, MsSqlConfiguration configuration, CancellationToken ct)
    {
        if (DefaultDatabase.Equals(configuration.Database))
        {
            return;
        }

        // The database name cannot be passed as the SQLCMDDBNAME scripting variable that
        // WithDatabase(string) sets. The -d option below overrides it with the default
        // database, which would resolve the statement against the wrong database.
        var database = configuration.Database;

        var sqlStatement = $"IF DB_ID('{database}') IS NULL BEGIN CREATE DATABASE [{database}] END";

        var sqlCmdFilePath = await container.GetSqlCmdFilePathAsync(ct)
            .ConfigureAwait(false);

        var readiness = new WaitUntil();

        // This is a simple workaround to use the default options, which can be configured
        // with custom configurations as needed.
        var options = new WaitStrategy();

        // The startup callback runs before any wait strategy. Waiting here prevents creating the
        // database while the MsSql instance is not accepting connections yet, which would
        // otherwise cause the container start to fail.
        await WaitStrategy.WaitUntilAsync(() => readiness.UntilAsync(container), options.Interval, options.Timeout, options.Retries, ct)
            .ConfigureAwait(false);

        _ = await container.ExecAsync(new[] { sqlCmdFilePath, "-C", "-b", "-r", "1", "-d", DefaultDatabase, "-Q", sqlStatement }, ct)
            .ThrowOnFailure()
            .ConfigureAwait(false);
    }

    /// <inheritdoc cref="IWaitUntil" />
    /// <remarks>
    /// Uses the <c>sqlcmd</c> utility scripting variables to detect readiness of the MsSql container:
    /// https://learn.microsoft.com/en-us/sql/tools/sqlcmd/sqlcmd-utility?view=sql-server-linux-ver15#sqlcmd-scripting-variables.
    /// </remarks>
    private sealed class WaitUntil : IWaitUntil
    {
        /// <inheritdoc />
        public Task<bool> UntilAsync(IContainer container)
        {
            return UntilAsync(container as MsSqlContainer);
        }

        /// <inheritdoc cref="IWaitUntil.UntilAsync" />
        private static async Task<bool> UntilAsync(MsSqlContainer container)
        {
            var sqlCmdFilePath = await container.GetSqlCmdFilePathAsync()
                .ConfigureAwait(false);

            var execResult = await container.ExecAsync(new[] { sqlCmdFilePath, "-C", "-b", "-r", "1", "-d", DefaultDatabase, "-Q", "SELECT 1;" })
                .ConfigureAwait(false);

            return 0L.Equals(execResult.ExitCode);
        }
    }
}