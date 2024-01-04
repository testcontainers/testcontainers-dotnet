namespace Testcontainers.Oracle;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OracleBuilder : ContainerBuilder<OracleBuilder, OracleContainer, OracleConfiguration>
{
    public const string OracleImage = "gvenzl/oracle-xe:21.3.0-slim-faststart";

    public const ushort OraclePort = 1521;

    public const string DefaultDatabase = "XEPDB1";

    public const string DefaultUsername = "oracle";

    public const string DefaultPassword = "oracle";

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleBuilder" /> class.
    /// </summary>
    public OracleBuilder()
        : this(new OracleConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OracleBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private OracleBuilder(OracleConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override OracleConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the Oracle username.
    /// </summary>
    /// <param name="username">The Oracle username.</param>
    /// <returns>A configured instance of <see cref="OracleBuilder" />.</returns>
    public OracleBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(username: username))
            .WithEnvironment("APP_USER", username);
    }

    /// <summary>
    /// Sets the Oracle password.
    /// </summary>
    /// <param name="password">The Oracle password.</param>
    /// <returns>A configured instance of <see cref="OracleBuilder" />.</returns>
    public OracleBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(password: password))
            .WithEnvironment("ORACLE_PASSWORD", password)
            .WithEnvironment("APP_USER_PASSWORD", password);
    }

    /// <inheritdoc />
    public override OracleContainer Build()
    {
        Validate();
        return new OracleContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override OracleBuilder Init()
    {
        return base.Init()
            .WithImage(OracleImage)
            .WithPortBinding(OraclePort, true)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("DATABASE IS READY TO USE!"));
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
    protected override OracleBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OracleBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override OracleBuilder Merge(OracleConfiguration oldValue, OracleConfiguration newValue)
    {
        return new OracleBuilder(new OracleConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Sets the Oracle database.
    /// </summary>
    /// <remarks>
    /// The Docker image does not allow to configure the database.
    /// </remarks>
    /// <param name="database">The Oracle database.</param>
    /// <returns>A configured instance of <see cref="OracleBuilder" />.</returns>
    private OracleBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(database: database));
    }
}