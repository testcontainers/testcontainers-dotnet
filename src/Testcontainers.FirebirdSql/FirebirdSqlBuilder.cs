namespace Testcontainers.FirebirdSql;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public class FirebirdSqlBuilder : ContainerBuilder<FirebirdSqlBuilder, FirebirdSqlContainer, FirebirdSqlConfiguration>
{
    public const string FirebirdSqlImage = "jacobalberty/firebird";

    public const ushort FirebirdSqlPort = 3050;

    public const string DefaultDatabase = "test";
    public const string DefaultUsername = "test";
    public const string DefaultPassword = "test";
    public const string FirebirdSysdba = "sysdba";
    public const string DefaultSysdbaPassword = "masterkey";

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlBuilder" /> class.
    /// </summary>
    public FirebirdSqlBuilder()
        : this(new FirebirdSqlConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirebirdSqlBuilder(FirebirdSqlConfiguration dockerResourceConfiguration)
        : base(dockerResourceConfiguration)
    {
        DockerResourceConfiguration = dockerResourceConfiguration;
    }

    /// <inheritdoc />
    protected override FirebirdSqlConfiguration DockerResourceConfiguration { get; }


    /// <summary>
    /// Sets the FirebirdSql username.
    /// </summary>
    /// <remarks>
    /// Can be set to 'sysdba'. In that case please use <see cref="WithSysdbaPassword" /> to set the password to connect with.
    /// </remarks>
    /// <param name="username">The FirebirdSql username.</param>
    /// <returns>A configured instance of <see cref="FirebirdSqlBuilder" />.</returns>
    public FirebirdSqlBuilder WithUsername(string username)
    {
        var builder = Merge(DockerResourceConfiguration, new(username: username));
        if (username.ToLower() != FirebirdSysdba)
        {
            builder = builder
                .WithEnvironment("FIREBIRD_USER", username);
        }

        return builder;
    }

    /// <summary>
    /// Sets the FirebirdSql password.
    /// </summary>
    /// <remarks>
    /// To authenticate with the configured user. If the user is 'sysdba' please use <see cref="WithSysdbaPassword" />
    /// </remarks>
    /// <param name="password">The FirebirdSql password.</param>
    /// <returns>A configured instance of <see cref="FirebirdSqlBuilder" />.</returns>
    public FirebirdSqlBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new(password: password))
            .WithEnvironment("FIREBIRD_PASSWORD", password);
    }

    /// <summary>
    /// Sets the FirebirdSql sysdba password.
    /// </summary>
    /// <remarks>
    /// To authenticate with the sysdba user. If the user is not 'sysdba' please use <see cref="WithPassword" />
    /// </remarks>
    /// <param name="sysdbaPassword">The FirebirdSql password.</param>
    /// <returns>A configured instance of <see cref="FirebirdSqlBuilder" />.</returns>
    public FirebirdSqlBuilder WithSysdbaPassword(string sysdbaPassword)
    {
        return Merge(DockerResourceConfiguration, new(sysdbaPassword: sysdbaPassword))
            .WithEnvironment("ISC_PASSWORD", sysdbaPassword);
    }

    /// <summary>
    /// Sets the FirebirdSql database.
    /// </summary>
    /// <param name="database">The FirebirdSql database.</param>
    /// <returns>A configured instance of <see cref="FirebirdSqlBuilder" />.</returns>
    public FirebirdSqlBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new(database: database))
            .WithEnvironment("FIREBIRD_DATABASE", database);
    }

    /// <inheritdoc />
    public override FirebirdSqlContainer Build()
    {
        Validate();
        var compoundWaitStrategy = Wait.ForUnixContainer()
            .UntilContainerIsHealthy()
            .AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration));
        return new FirebirdSqlContainer(
            WithWaitStrategy(compoundWaitStrategy).DockerResourceConfiguration,
            TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override FirebirdSqlBuilder Init()
        => base.Init()
            .WithImage(FirebirdSqlImage)
            .WithPortBinding(FirebirdSqlPort, true)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithSysdbaPassword(DefaultSysdbaPassword)
            .WithResourceMapping(Encoding.UTF8.GetBytes(FirebirdSqlContainer.TestQueryString), "/home/firebird_check.sql");

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override FirebirdSqlBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new(resourceConfiguration));

    /// <inheritdoc />
    protected override FirebirdSqlBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new(resourceConfiguration));

    /// <inheritdoc />
    protected override FirebirdSqlBuilder Merge(FirebirdSqlConfiguration oldValue, FirebirdSqlConfiguration newValue)
        => new(new(oldValue, newValue));

    /// <inheritdoc cref="IWaitUntil" />
    /// <remarks>
    /// Uses the isql Firebird Interactive SQL Utility to detect the readiness of the FirebirdSql container:
    /// https://www.firebirdsql.org/file/documentation/html/en/firebirddocs/isql/firebird-isql.html.
    /// </remarks>
    private sealed class WaitUntil(FirebirdSqlConfiguration configuration) : IWaitUntil
    {
        private readonly string[] checkDatabaseCommand =
        {
            "/usr/local/firebird/bin/isql",
            "-i",
            "/home/firebird_check.sql",
            $"localhost:{configuration.Database}",
            "-user",
            configuration.Username,
            "-pass",
            configuration.Password,
        };

        public async Task<bool> UntilAsync(IContainer container)
        {
            var executionResult = await container.ExecAsync(checkDatabaseCommand)
                .ConfigureAwait(false);
            return 0L.Equals(executionResult.ExitCode);
        }
    }
}
