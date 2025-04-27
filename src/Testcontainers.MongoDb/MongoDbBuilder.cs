namespace Testcontainers.MongoDb;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MongoDbBuilder : ContainerBuilder<MongoDbBuilder, MongoDbContainer, MongoDbConfiguration>
{
    public const string MongoDbImage = "mongo:6.0";

    public const ushort MongoDbPort = 27017;

    public const string DefaultUsername = "mongo";

    public const string DefaultPassword = "mongo";

    private const string InitKeyFileScriptFilePath = "/docker-entrypoint-initdb.d/01-init-keyfile.sh";

    private const string KeyFileFilePath = "/tmp/mongodb-keyfile";

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

    /// <summary>
    /// Initialize MongoDB as a single-node replica set.
    /// </summary>
    /// <param name="replicaSetName">The replica set name.</param>
    /// <returns>A configured instance of <see cref="MongoDbBuilder" />.</returns>
    public MongoDbBuilder WithReplicaSet(string replicaSetName = "rs0")
    {
        var initKeyFileScript = new StringWriter();
        initKeyFileScript.NewLine = "\n";
        initKeyFileScript.WriteLine("#!/bin/bash");
        initKeyFileScript.WriteLine("openssl rand -base64 32 > \"" + KeyFileFilePath + "\"");
        initKeyFileScript.WriteLine("chmod 600 \"" + KeyFileFilePath + "\"");

        return Merge(DockerResourceConfiguration, new MongoDbConfiguration(replicaSetName: replicaSetName))
            .WithCommand("--replSet", replicaSetName, "--keyFile", KeyFileFilePath, "--bind_ip_all")
            .WithResourceMapping(Encoding.Default.GetBytes(initKeyFileScript.ToString()), InitKeyFileScriptFilePath, Unix.FileMode755);
    }

    /// <inheritdoc />
    public override MongoDbContainer Build()
    {
        Validate();

        IWaitUntil waitUntil;

        if (string.IsNullOrEmpty(DockerResourceConfiguration.ReplicaSetName))
        {
            // The wait strategy relies on the configuration of MongoDb. If credentials are
            // provided, the log message "Waiting for connections" appears twice.
            waitUntil = new WaitIndicateReadiness(DockerResourceConfiguration);
        }
        else
        {
            waitUntil = new WaitInitiateReplicaSet(DockerResourceConfiguration);
        }

        // If the user does not provide a custom waiting strategy, append the default MongoDb waiting strategy.
        var mongoDbBuilder = DockerResourceConfiguration.WaitStrategies.Count() > 1 ? this : WithWaitStrategy(Wait.ForUnixContainer().AddCustomWaitStrategy(waitUntil));
        return new MongoDbContainer(mongoDbBuilder.DockerResourceConfiguration);
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
    private sealed class WaitIndicateReadiness : IWaitUntil
    {
        private static readonly string[] LineEndings = { "\r\n", "\n" };

        private readonly int _count;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitIndicateReadiness" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitIndicateReadiness(MongoDbConfiguration configuration)
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

    /// <inheritdoc cref="IWaitUntil" />
    private sealed class WaitInitiateReplicaSet : IWaitUntil
    {
        private readonly string _scriptContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaitInitiateReplicaSet" /> class.
        /// </summary>
        /// <param name="configuration">The container configuration.</param>
        public WaitInitiateReplicaSet(MongoDbConfiguration configuration)
        {
            _scriptContent = $"try{{rs.status()}}catch(e){{rs.initiate({{_id:'{configuration.ReplicaSetName}',members:[{{_id:0,host:'127.0.0.1:27017'}}]}});throw e;}}";
        }

        /// <inheritdoc />
        public Task<bool> UntilAsync(IContainer container)
        {
            return UntilAsync(container as MongoDbContainer);
        }

        /// <inheritdoc cref="IWaitUntil.UntilAsync" />
        private async Task<bool> UntilAsync(MongoDbContainer container)
        {
            var execResult = await container.ExecScriptAsync(_scriptContent)
                .ConfigureAwait(false);

            return 0L.Equals(execResult.ExitCode);
        }
    }
}