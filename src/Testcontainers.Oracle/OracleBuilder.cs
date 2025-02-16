namespace Testcontainers.Oracle;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class OracleBuilder : ContainerBuilder<OracleBuilder, OracleContainer, OracleConfiguration>
{
    public const string OracleImage = "gvenzl/oracle-xe:21.3.0-slim-faststart";

    public const ushort OraclePort = 1521;

    [Obsolete("This constant is obsolete and should not be used. It is only applicable for Oracle images between versions 11 and 22.")]
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

        const string message = "The image '{0}' does not support configuring the database. It is only supported on Oracle 18 and onwards.";

        Predicate<OracleConfiguration> databaseConfigurationNotSupported = value =>
            value.Database != null && value.Image.MatchVersion(v => v.Major < 18);

        _ = Guard.Argument(DockerResourceConfiguration, nameof(DockerResourceConfiguration.Database))
            .ThrowIf(argument => databaseConfigurationNotSupported(argument.Value), _ => throw new NotSupportedException(string.Format(message, DockerResourceConfiguration.Image.FullName)));

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull()
            .NotEmpty();

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

    private string GetDefaultServiceName()
    {
        if (DockerResourceConfiguration.Image.MatchVersion(v => v.Major >= 23))
        {
            return "FREEPDB1";
        }

        if (DockerResourceConfiguration.Image.MatchVersion(v => v.Major > 11))
        {
            return "XEPDB1";
        }

        return "XE";
    }
}