namespace Testcontainers.Db2;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class Db2Builder : ContainerBuilder<Db2Builder, Db2Container, Db2Configuration>
{
    public const string Db2Image = "icr.io/db2_community/db2:12.1.0.0";

    public const ushort Db2Port = 50000;

    public const string DefaultDatabase = "test";

    public const string DefaultUsername = "db2inst1";

    public const string DefaultPassword = "db2inst1";

    /// <summary>
    /// Initializes a new instance of the <see cref="Db2Builder" /> class.
    /// </summary>
    public Db2Builder()
        : this(new Db2Configuration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Db2Builder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private Db2Builder(Db2Configuration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override Db2Configuration DockerResourceConfiguration { get; }

    /// <inheritdoc />
    protected override string AcceptLicenseAgreementEnvVar { get; } = "LICENSE";

    /// <inheritdoc />
    protected override string AcceptLicenseAgreement { get; } = "accept";

    /// <inheritdoc />
    protected override string DeclineLicenseAgreement { get; } = "decline";

    /// <summary>
    /// Accepts the license agreement.
    /// </summary>
    /// <remarks>
    /// When <paramref name="acceptLicenseAgreement" /> is set to <c>true</c>, the Db2 <see href="www.ibm.com/terms/?id=L-SNMD-UVTL8R">license</see> is accepted.
    /// </remarks>
    /// <param name="acceptLicenseAgreement">A boolean value indicating whether the Db2 license agreement is accepted.</param>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public override Db2Builder WithAcceptLicenseAgreement(bool acceptLicenseAgreement)
    {
        var licenseAgreement = acceptLicenseAgreement ? AcceptLicenseAgreement : DeclineLicenseAgreement;
        return WithEnvironment(AcceptLicenseAgreementEnvVar, licenseAgreement);
    }

    /// <summary>
    /// Sets the Db2 database name.
    /// </summary>
    /// <param name="database">The Db2 database.</param>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public Db2Builder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(database: database))
            .WithEnvironment("DBNAME", database);
    }

    /// <summary>
    /// Sets the Db2 username.
    /// </summary>
    /// <param name="username">The Db2 username.</param>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public Db2Builder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(username: username))
            .WithEnvironment("DB2INSTANCE", username)
            .WithTmpfsMount(string.Join("/", string.Empty, "home", username, "data"));
    }

    /// <summary>
    /// Sets the Db2 password.
    /// </summary>
    /// <param name="password">The Db2 password.</param>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public Db2Builder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(password: password))
            .WithEnvironment("DB2INST1_PASSWORD", password);
    }

    /// <inheritdoc />
    public override Db2Container Build()
    {
        Validate();
        ValidateLicenseAgreement();
        return new Db2Container(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override Db2Builder Init() => base.Init()
        .WithImage(Db2Image)
        .WithPortBinding(Db2Port, true)
        .WithDatabase(DefaultDatabase)
        .WithUsername(DefaultUsername)
        .WithPassword(DefaultPassword)
        .WithPrivileged(true)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged("Setup has completed."));

    /// <inheritdoc />
    protected override void Validate()
    {
        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull()
            .NotEmpty();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull()
            .NotEmpty();
    }

    /// <inheritdoc />
    protected override Db2Builder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Db2Builder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override Db2Builder Merge(Db2Configuration oldValue, Db2Configuration newValue)
    {
        return new Db2Builder(new Db2Configuration(oldValue, newValue));
    }
}