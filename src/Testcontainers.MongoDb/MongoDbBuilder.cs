namespace Testcontainers.MongoDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MongoDbBuilder : ContainerBuilder<MongoDbBuilder, MongoDbContainer, MongoDbConfiguration>
{
    public const string MongoDbImage = "mongo:6.0";

    public const ushort MongoDbPort = 27017;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbBuilder" /> class.
    /// </summary>
    public MongoDbBuilder()
        : this(new MongoDbConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MongoDbBuilder(MongoDbConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MongoDbConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the MongoDb username.
    /// </summary>
    /// <param name="username">The MongoDb username.</param>
    /// <returns>A configured instance of <see cref="MongoDbBuilder" />.</returns>
    public MongoDbBuilder WithUsername(string username)
    {
        return Merge(DockerResourceConfiguration, new MongoDbConfiguration(username: username))
            .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", username);
    }

    /// <summary>
    /// Sets the MongoDb password.
    /// </summary>
    /// <param name="password">The MongoDb password.</param>
    /// <returns>A configured instance of <see cref="MongoDbBuilder" />.</returns>
    public MongoDbBuilder WithPassword(string password)
    {
        return Merge(DockerResourceConfiguration, new MongoDbConfiguration(password: password))
            .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", password);
    }

    /// <inheritdoc />
    public override MongoDbContainer Build()
    {
        Validate();

        var mongoDbBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new MongoDbContainer(mongoDbBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override MongoDbBuilder Init()
    {
        return base.Init()
            .WithImage(MongoDbImage)
            .WithPortBinding(MongoDbPort, true)
            .WithUsername("mongo")
            .WithPassword(Guid.NewGuid().ToString("D"));
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
    protected override MongoDbBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MongoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MongoDbBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MongoDbConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MongoDbBuilder Merge(MongoDbConfiguration oldValue, MongoDbConfiguration newValue)
    {
        return new MongoDbBuilder(new MongoDbConfiguration(oldValue, newValue));
    }

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitUntil : IWaitUntil
    {
        private readonly IList<string> _mongoDbShellCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(MongoDbConfiguration configuration)
        {
            const string js = "db.runCommand({hello:1}).isWritablePrimary";
            _mongoDbShellCommand = new MongoDbShellCommand(js, configuration.Username, configuration.Password);
        }

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var execResult = await container.ExecAsync(_mongoDbShellCommand)
                .ConfigureAwait(false);

            return 0L.Equals(execResult.ExitCode) && "true\n".Equals(execResult.Stdout, StringComparison.OrdinalIgnoreCase);
        }
    }
}