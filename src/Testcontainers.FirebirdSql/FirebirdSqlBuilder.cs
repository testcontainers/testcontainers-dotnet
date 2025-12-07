namespace Testcontainers.FirebirdSql;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class FirebirdSqlBuilder : ContainerBuilder<FirebirdSqlBuilder, FirebirdSqlContainer, FirebirdSqlConfiguration>
{
    [Obsolete("This image tag is not recommended: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public const string FirebirdSqlImage = "jacobalberty/firebird:v4.0";

    public const ushort FirebirdSqlPort = 3050;

    public const string DefaultDatabase = "test";

    public const string DefaultUsername = "test";

    public const string DefaultPassword = "test";

    public const string DefaultSysdbaPassword = "masterkey";

    private const string TestQueryString = "SELECT 1 FROM RDB$DATABASE;";

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlBuilder" /> class.
    /// </summary>
    [Obsolete("Use the constructor with the image argument instead: https://github.com/testcontainers/testcontainers-dotnet/issues/1540.")]
    public FirebirdSqlBuilder()
        : this(FirebirdSqlImage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>jacobalberty/firebird:v4.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/firebirdsql/firebird/tags" />.
    /// </remarks>
    public FirebirdSqlBuilder(string image)
        : this(new FirebirdSqlConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlBuilder" /> class.
    /// </summary>
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/firebirdsql/firebird/tags" />.
    /// </remarks>
    public FirebirdSqlBuilder(IImage image)
        : this(new FirebirdSqlConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FirebirdSqlBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    public FirebirdSqlBuilder(FirebirdSqlConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override FirebirdSqlConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the FirebirdSql database.
    /// </summary>
    /// <param name="database">The FirebirdSql database.</param>
    /// <returns>A configured instance of <see cref="FirebirdSqlBuilder" />.</returns>
    public FirebirdSqlBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new FirebirdSqlConfiguration(database: database))
            .WithEnvironment("FIREBIRD_DATABASE", database);
    }

    /// <summary>
    /// Sets the FirebirdSql username.
    /// </summary>
    /// <param name="username">The FirebirdSql username.</param>
    /// <returns>A configured instance of <see cref="FirebirdSqlBuilder" />.</returns>
    public FirebirdSqlBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new FirebirdSqlConfiguration(username: username))
            .WithEnvironment("FIREBIRD_USER", "sysdba".Equals(username, StringComparison.OrdinalIgnoreCase) ? string.Empty : username);
    }

    /// <summary>
    /// Sets the FirebirdSql password.
    /// </summary>
    /// <param name="password">The FirebirdSql password.</param>
    /// <returns>A configured instance of <see cref="FirebirdSqlBuilder" />.</returns>
    public FirebirdSqlBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new FirebirdSqlConfiguration(password: password))
            .WithEnvironment("FIREBIRD_PASSWORD", password)
            .WithEnvironment("ISC_PASSWORD", password);
    }

    /// <inheritdoc />
    public override FirebirdSqlContainer Build()
    {
        Validate();
        return new FirebirdSqlContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override FirebirdSqlBuilder Init()
    {
        return base.Init()
            .WithPortBinding(FirebirdSqlPort, true)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithResourceMapping(Encoding.Default.GetBytes(TestQueryString), "/home/firebird_check.sql")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
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
    protected override FirebirdSqlBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FirebirdSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FirebirdSqlBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new FirebirdSqlConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override FirebirdSqlBuilder Merge(FirebirdSqlConfiguration oldValue, FirebirdSqlConfiguration newValue)
    {
        return new FirebirdSqlBuilder(new FirebirdSqlConfiguration(oldValue, newValue));
    }
}