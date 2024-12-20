namespace Testcontainers.Oracle;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OracleBuilder : ContainerBuilder<OracleBuilder, OracleContainer, OracleConfiguration>
{
    public const string OracleImage = "gvenzl/oracle-xe:21.3.0-slim-faststart";

    public const ushort OraclePort = 1521;

    [Obsolete("Not used anymore. Only valid for Oracle images > 11 and < 23")]
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

    /// <summary>
    /// Sets the Oracle database.
    /// </summary>
    /// <remarks>
    /// The database can only be set for Oracle 18 and onwards.
    /// </remarks>
    /// <param name="database">The Oracle database.</param>
    /// <returns>A configured instance of <see cref="OracleBuilder" />.</returns>
    public OracleBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new OracleConfiguration(database: database));
    }

    /// <inheritdoc />
    public override OracleContainer Build()
    {
        Validate();

        var defaultServiceName = GetDefaultServiceName();
        if (DockerResourceConfiguration.Database == null)
        {
            return new OracleContainer(WithDatabase(defaultServiceName).DockerResourceConfiguration);
        }

        if (DockerResourceConfiguration.Database != defaultServiceName)
        {
            return new OracleContainer(WithEnvironment("ORACLE_DATABASE", DockerResourceConfiguration.Database).DockerResourceConfiguration);
        }

        return new OracleContainer(DockerResourceConfiguration);
    }

    private string GetDefaultServiceName()
    {
        if (DockerResourceConfiguration.Image.MatchVersion(v => v.Major >= 23))
            return "FREEPDB1";

        if (DockerResourceConfiguration.Image.MatchVersion(v => v.Major > 11))
            return "XEPDB1";

        return "XE";
    }

    /// <inheritdoc />
    protected override OracleBuilder Init()
    {
        return base.Init()
            .WithImage(OracleImage)
            .WithPortBinding(OraclePort, true)
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

        if (DockerResourceConfiguration.Database != null && DockerResourceConfiguration.Image.MatchVersion(v => v.Major < 18))
            throw new NotSupportedException($"Setting the database is not supported with {DockerResourceConfiguration.Image.FullName}. It is only supported on Oracle 18 and onwards.");
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
}