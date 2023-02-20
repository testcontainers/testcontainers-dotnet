namespace Testcontainers.MariaDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MariaDbBuilder : ContainerBuilder<MariaDbBuilder, MariaDbContainer, MariaDbConfiguration>
{
    public const string MariaDbImage = "mariadb:10.10";

    public const ushort MariaDbPort = 3306;

    public const string DefaultDatabase = "mariadb";

    public const string DefaultUsername = "mariadb";

    public const string DefaultPassword = "mariadb";

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbBuilder" /> class.
    /// </summary>
    public MariaDbBuilder()
        : this(new MariaDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MariaDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MariaDbBuilder(MariaDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MariaDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the MariaDb database.
    /// </summary>
    /// <param name="database">The MariaDb database.</param>
    /// <returns>A configured instance of <see cref="MariaDbBuilder" />.</returns>
    public MariaDbBuilder WithDatabase(string database)
    {
        return Merge(DockerResourceConfiguration, new MariaDbConfiguration(database: database))
            .WithEnvironment("MARIADB_DATABASE", database);
    }

    /// <summary>
    /// Sets the MariaDb username.
    /// </summary>
    /// <param name="username">The MariaDb username.</param>
    /// <returns>A configured instance of <see cref="MariaDbBuilder" />.</returns>
    public MariaDbBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new MariaDbConfiguration(username: username))
            .WithEnvironment("MARIADB_USER", "root".Equals(username, StringComparison.OrdinalIgnoreCase) ? string.Empty : username);
    }

    /// <summary>
    /// Sets the MariaDb password.
    /// </summary>
    /// <param name="password">The MariaDb password.</param>
    /// <returns>A configured instance of <see cref="MariaDbBuilder" />.</returns>
    public MariaDbBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new MariaDbConfiguration(password: password))
            .WithEnvironment("MARIADB_PASSWORD", password)
            .WithEnvironment("MARIADB_ROOT_PASSWORD", password);
    }

    /// <inheritdoc />
    public override MariaDbContainer Build()
    {
        Validate();

        // By default, the base builder waits until the container is running. However, for MariaDb, a more advanced waiting strategy is necessary that requires access to the configured database, username and password.
        // If the user does not provide a custom waiting strategy, append the default MariaDb waiting strategy.
        var mariaDbBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new MariaDbContainer(mariaDbBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override MariaDbBuilder Init()
    {
        return base.Init()
            .WithImage(MariaDbImage)
            .WithPortBinding(MariaDbPort, true)
            .WithDatabase(DefaultDatabase)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword);
    }

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
    protected override MariaDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MariaDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MariaDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MariaDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MariaDbBuilder Merge(MariaDbConfiguration oldValue, MariaDbConfiguration newValue)
    {
        return new MariaDbBuilder(new MariaDbConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly IList<string> _command;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(MariaDbConfiguration configuration)
        {
            _command = new List<string> { "mysql", "--protocol=TCP", $"--port={MariaDbPort}", $"--user={configuration.Username}", $"--password={configuration.Password}", configuration.Database, "--wait", "--silent", "--execute=SELECT 1;" };
        }

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var execResult = await container.ExecAsync(_command)
                .ConfigureAwait(false);

            return 0L.Equals(execResult.ExitCode);
        }
    }
}