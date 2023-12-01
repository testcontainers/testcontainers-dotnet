namespace Testcontainers.SqlEdge;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class SqlEdgeBuilder : ContainerBuilder<SqlEdgeBuilder, SqlEdgeContainer, SqlEdgeConfiguration>
{
    public const string SqlEdgeImage = "mcr.microsoft.com/azure-sql-edge:1.0.7";

    public const ushort SqlEdgePort = 1433;

    public const string DefaultDatabase = "master";

    public const string DefaultUsername = "sa";

    public const string DefaultPassword = "yourStrong(!)Password";

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeBuilder" /> class.
    /// </summary>
    public SqlEdgeBuilder()
        : this(new SqlEdgeConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlEdgeBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private SqlEdgeBuilder(SqlEdgeConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override SqlEdgeConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the SqlEdge password.
    /// </summary>
    /// <param name="password">The SqlEdge password.</param>
    /// <returns>A configured instance of <see cref="SqlEdgeBuilder" />.</returns>
    public SqlEdgeBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new SqlEdgeConfiguration(password: password))
            .WithEnvironment("MSSQL_SA_PASSWORD", password);
    }

    /// <inheritdoc />
    public override SqlEdgeContainer Build()
    {
        Validate();
        return new SqlEdgeContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override SqlEdgeBuilder Init()
    {
        return base.Init()
            .WithImage(SqlEdgeImage)
            .WithPortBinding(SqlEdgePort, true)
            .WithEnvironment("ACCEPT_EULA", "Y")
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Recovery is complete."));
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
    protected override SqlEdgeBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SqlEdgeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SqlEdgeBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new SqlEdgeConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override SqlEdgeBuilder Merge(SqlEdgeConfiguration oldValue, SqlEdgeConfiguration newValue)
    {
        return new SqlEdgeBuilder(new SqlEdgeConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Sets the SqlEdge database.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the database.
    /// </remarks>
    /// <param name="database">The SqlEdge database.</param>
    /// <returns>A configured instance of <see cref="SqlEdgeBuilder" />.</returns>
    private SqlEdgeBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new SqlEdgeConfiguration(database: database));
    }

    /// <summary>
    /// Sets the SqlEdge username.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the username.
    /// </remarks>
    /// <param name="username">The SqlEdge username.</param>
    /// <returns>A configured instance of <see cref="SqlEdgeBuilder" />.</returns>
    private SqlEdgeBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new SqlEdgeConfiguration(username: username));
    }
}