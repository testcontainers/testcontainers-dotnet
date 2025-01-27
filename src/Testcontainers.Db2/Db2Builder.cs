namespace Testcontainers.Db2;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class Db2Builder : ContainerBuilder<Db2Builder, Db2Container, Db2Configuration>
{
    public const string Db2Image = "icr.io/db2_community/db2:latest";

    public const ushort Db2Port = 50000;

    public const string DefaultDatabase = "test";

    public const string DefaultUsername = "db2inst1";

    public const string DefaultPassword = "db2inst1";

    public const string DefaultLicenseAgreement = "accept";

    public const string DefaultInMemoryDatabasePath = "/home/db2inst1/data";

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
            .WithEnvironment("DB2INSTANCE", username);
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

    /// <summary>
    /// Sets the Db2 archive logs.
    /// </summary>
    /// <param name="archiveLogs">The Db2 archive logs setting.</param>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public Db2Builder WithArchiveLogs(bool archiveLogs)
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(archiveLogs: archiveLogs))
            .WithEnvironment("ARCHIVE_LOGS", archiveLogs.ToString());
    }

    /// <summary>
    /// Sets the Db2 autoconfig setting.
    /// </summary>
    /// <param name="autoConfig">The Db2 autoconfig setting.</param>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public Db2Builder WithAutoconfig(bool autoConfig)
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(autoConfig: autoConfig))
            .WithEnvironment("AUTOCONFIG", autoConfig.ToString());
    }

    /// <summary>
    /// Accepts the Db2 license agreement.
    /// </summary>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public Db2Builder WithLicenseAgreement()
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(licenseAgreement: DefaultLicenseAgreement))
            .WithEnvironment("LICENSE", DefaultLicenseAgreement);
    }

    /// <summary>
    /// Maps the database to memory.
    /// </summary>
    /// <returns>A configured instance of <see cref="Db2Builder" />.</returns>
    public Db2Builder WithInMemoryDatabase()
    {
        return Merge(DockerResourceConfiguration, new Db2Configuration(licenseAgreement: DefaultInMemoryDatabasePath))
            .WithTmpfsMount(DefaultInMemoryDatabasePath);
    }

    /// <inheritdoc />
    public override Db2Container Build()
    {
        Validate();

        // By default, the base builder waits until the container is running. However, for Db2, a more advanced waiting strategy is necessary
        // If the user does not provide a custom waiting strategy, append the default Db2 waiting strategy.
        var db2Builder = DockerResourceConfiguration.WaitStrategies.Count() > 1
            ? this
            : WithWaitStrategy(Wait.ForUnixContainer()
                .UntilMessageIsLogged("All databases are now active")
                .UntilMessageIsLogged("Setup has completed.")
                .AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration))
            );

        return new Db2Container(db2Builder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override Db2Builder Init() => base.Init()
        .WithImage(Db2Image)
        .WithPortBinding(Db2Port, true)
        .WithDatabase(DefaultDatabase)
        .WithUsername(DefaultUsername)
        .WithPassword(DefaultPassword)
        .WithLicenseAgreement()
        .WithArchiveLogs(false)
        .WithAutoconfig(false)
        .WithInMemoryDatabase()
        .WithPrivileged(true);

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

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(Db2Configuration configuration)
        {
        }

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var db2Container = (Db2Container)container;

            var execResult = await db2Container.ExecScriptAsync("SELECT 1 FROM SYSIBM.SYSDUMMY1").ConfigureAwait(false);

            return 0L.Equals(execResult.ExitCode);
        }
    }
}