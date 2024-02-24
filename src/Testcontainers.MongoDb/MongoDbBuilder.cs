namespace Testcontainers.MongoDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MongoDbBuilder : ContainerBuilder<MongoDbBuilder, MongoDbContainer, MongoDbConfiguration>
{
    public const string MongoDbImage = "mongo:6.0";

    public const ushort MongoDbPort = 27017;

    public const string DefaultUsername = "mongo";

    public const string DefaultPassword = "mongo";

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
        var initDbRootUsername = username ?? string.Empty;

        return Merge(DockerResourceConfiguration, new MongoDbConfiguration(username: initDbRootUsername))
            .WithEnvironment("MONGO_INITDB_ROOT_USERNAME", initDbRootUsername);
    }

    /// <summary>
    /// Sets the MongoDb password.
    /// </summary>
    /// <param name="password">The MongoDb password.</param>
    /// <returns>A configured instance of <see cref="MongoDbBuilder" />.</returns>
    public MongoDbBuilder WithPassword(string password)
    {
        var initDbRootPassword = password ?? string.Empty;

        return Merge(DockerResourceConfiguration, new MongoDbConfiguration(password: initDbRootPassword))
            .WithEnvironment("MONGO_INITDB_ROOT_PASSWORD", initDbRootPassword);
    }

    /// <inheritdoc />
    public override MongoDbContainer Build()
    {
        Validate();

        // The wait strategy relies on the configuration of MongoDb. If credentials are
        // provided, the log message "Waiting for connections" appears twice.
        // If the user does not provide a custom waiting strategy, append the default MongoDb waiting strategy.
        var mongoDbBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(new WaitUntil(DockerResourceConfiguration)));
        return new MongoDbContainer(mongoDbBuilder.DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    /// <inheritdoc />
    protected override MongoDbBuilder Init()
    {
        return base.Init()
            .WithImage(MongoDbImage)
            .WithPortBinding(MongoDbPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string message = "Missing username or password. Both must be specified for a user to be created.";

        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull();

        _ = Guard.Argument(DockerResourceConfiguration, "Credentials")
            .ThrowIf(argument => 1.Equals(new[] { argument.Value.Username, argument.Value.Password }.Count(string.IsNullOrWhiteSpace)), argument => new ArgumentException(message, argument.Name));
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
        private static readonly string[] LineEndings = { "\r\n", "\n" };

        private readonly int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitUntil" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitUntil(MongoDbConfiguration configuration)
        {
            _count = string.IsNullOrEmpty(configuration.Username) && string.IsNullOrEmpty(configuration.Password) ? 1 : 2;
        }

        /// <inheritdoc />
        public async Task<bool> UntilAsync(IContainer container)
        {
            var (stdout, stderr) = await container.GetLogsAsync(since: container.StoppedTime, timestampsEnabled: false)
                .ConfigureAwait(false);

            return _count.Equals(Array.Empty<string>()
                .Concat(stdout.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries))
                .Concat(stderr.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries))
                .Count(line => line.Contains("Waiting for connections")));
        }
    }
}