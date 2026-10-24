namespace Testcontainers.MongoDbAtlasLocal;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MongoDbAtlasLocalBuilder : ContainerBuilder<MongoDbAtlasLocalBuilder, MongoDbAtlasLocalContainer, MongoDbAtlasLocalConfiguration>
{
    public const ushort MongoDbAtlasLocalPort = 27017;

    public const string DefaultUsername = "mongo";

    public const string DefaultPassword = "mongo";

    private const string InitScriptsDirectoryPath = "/docker-entrypoint-initdb.d/";

    private static readonly string[] InitScriptFileExtensions = { ".js", ".sh" };

    private static readonly char[] PathSeparators = { '/', '\\' };

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>mongodb/mongodb-atlas-local:8.0</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/mongodb/mongodb-atlas-local/tags" />.
    /// </remarks>
    public MongoDbAtlasLocalBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/mongodb/mongodb-atlas-local/tags" />.
    /// </remarks>
    public MongoDbAtlasLocalBuilder(IImage image)
        : this(new MongoDbAtlasLocalConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MongoDbAtlasLocalBuilder(MongoDbAtlasLocalConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MongoDbAtlasLocalConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the MongoDb Atlas Local username.
    /// </summary>
    /// <param name="username">The MongoDb Atlas Local username.</param>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    public MongoDbAtlasLocalBuilder WithUsername(string username)
    {
        var initDbRootUsername = username ?? string.Empty;

        return Merge(DockerResourceConfiguration, new MongoDbAtlasLocalConfiguration(username: initDbRootUsername))
            .WithEnvironment("MONGODB_INITDB_ROOT_USERNAME", initDbRootUsername);
    }

    /// <summary>
    /// Sets the MongoDb Atlas Local password.
    /// </summary>
    /// <param name="password">The MongoDb Atlas Local password.</param>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    public MongoDbAtlasLocalBuilder WithPassword(string password)
    {
        var initDbRootPassword = password ?? string.Empty;

        return Merge(DockerResourceConfiguration, new MongoDbAtlasLocalConfiguration(password: initDbRootPassword))
            .WithEnvironment("MONGODB_INITDB_ROOT_PASSWORD", initDbRootPassword);
    }

    /// <summary>
    /// Sets the database the JavaScript init scripts run against.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>test</c>.
    /// </remarks>
    /// <param name="database">The init database.</param>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    public MongoDbAtlasLocalBuilder WithInitDatabase(string database)
    {
        return WithEnvironment("MONGODB_INITDB_DATABASE", database);
    }

    /// <summary>
    /// Copies an init script to the container to seed the deployment on its first start.
    /// </summary>
    /// <remarks>
    /// JavaScript (<c>.js</c>) scripts run in <c>mongosh</c> against the init database, shell
    /// (<c>.sh</c>) scripts run in <c>bash</c>. The scripts run in alphabetical order of their
    /// file names before the container is ready. A restarted or reused container keeps its
    /// data and does not run the scripts again.
    /// </remarks>
    /// <param name="scriptFilePath">The host file path of the init script.</param>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    public MongoDbAtlasLocalBuilder WithInitScript(string scriptFilePath)
    {
        return Merge(DockerResourceConfiguration, new MongoDbAtlasLocalConfiguration(initScriptFilePaths: new[] { scriptFilePath }));
    }

    /// <summary>
    /// Creates an init script in the container to seed the deployment on its first start.
    /// </summary>
    /// <remarks>
    /// See <see cref="WithInitScript(string)" /> for how the init scripts run.
    /// </remarks>
    /// <param name="fileName">The file name of the init script (e.g., <c>01-seed.js</c>).</param>
    /// <param name="scriptContent">The content of the init script.</param>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    public MongoDbAtlasLocalBuilder WithInitScriptContent(string fileName, string scriptContent)
    {
        var initScriptContents = new[] { new KeyValuePair<string, string>(fileName, scriptContent) };
        return Merge(DockerResourceConfiguration, new MongoDbAtlasLocalConfiguration(initScriptContents: initScriptContents));
    }

    /// <inheritdoc />
    public override MongoDbAtlasLocalContainer Build()
    {
        Validate();

        // The init scripts are mapped after the validation. Creating the resource
        // mappings in the builder methods would throw for invalid arguments before
        // Validate() can report them.
        var mongoDbAtlasLocalBuilder = DockerResourceConfiguration.InitScriptFilePaths
            .Aggregate(this, (builder, scriptFilePath) =>
                builder.WithResourceMapping(FilePath.Of(scriptFilePath), DirectoryPath.Of(InitScriptsDirectoryPath)));

        mongoDbAtlasLocalBuilder = DockerResourceConfiguration.InitScriptContents
            .Aggregate(mongoDbAtlasLocalBuilder, (builder, initScriptContent) =>
                builder.WithResourceMapping(Encoding.Default.GetBytes(initScriptContent.Value), FilePath.Of(InitScriptsDirectoryPath + initScriptContent.Key)));

        return new MongoDbAtlasLocalContainer(mongoDbAtlasLocalBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MongoDbAtlasLocalBuilder Init()
    {
        // The runner's healthcheck checks mongod, the replica set, mongot (the Atlas Search
        // process) and that the init scripts have finished. We run it directly instead of
        // waiting for the image's HEALTHCHECK, whose first probe runs 30 seconds after start.
        return base.Init()
            .WithPortBinding(MongoDbAtlasLocalPort, true)
            .WithUsername(DefaultUsername)
            .WithPassword(DefaultPassword)
            .WithEnvironment("DO_NOT_TRACK", "1")
            .WithConnectionStringProvider(() => new MongoDbAtlasLocalConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("runner", "healthcheck"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string credentialsMessage = "Missing username or password. Both must be specified for a user to be created.";

        const string initScriptFileNameMessage = "The init script file name must end with .js or .sh and cannot contain a path separator.";

        const string initScriptContentMessage = "The init script content cannot be null.";

        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration.Username, nameof(DockerResourceConfiguration.Username))
            .NotNull();

        _ = Guard.Argument(DockerResourceConfiguration.Password, nameof(DockerResourceConfiguration.Password))
            .NotNull();

        _ = Guard.Argument(DockerResourceConfiguration, "Credentials")
            .ThrowIf(argument => 1.Equals(new[] { argument.Value.Username, argument.Value.Password }.Count(string.IsNullOrWhiteSpace)), argument => new ArgumentException(credentialsMessage, argument.Name));

        // The runner skips files in the init scripts directory that do not end with
        // .js or .sh (case-sensitive) without an error.
        Predicate<string> isInvalidInitScriptFileName = value =>
            string.IsNullOrEmpty(value) || value.IndexOfAny(PathSeparators) >= 0 || !InitScriptFileExtensions.Any(fileExtension => value.EndsWith(fileExtension, StringComparison.Ordinal));

        _ = Guard.Argument(DockerResourceConfiguration.InitScriptFilePaths, nameof(DockerResourceConfiguration.InitScriptFilePaths))
            .ThrowIf(argument => argument.Value.Any(scriptFilePath => isInvalidInitScriptFileName(Path.GetFileName(scriptFilePath))), argument => new ArgumentException(initScriptFileNameMessage, argument.Name));

        _ = Guard.Argument(DockerResourceConfiguration.InitScriptContents, nameof(DockerResourceConfiguration.InitScriptContents))
            .ThrowIf(argument => argument.Value.Any(initScriptContent => isInvalidInitScriptFileName(initScriptContent.Key)), argument => new ArgumentException(initScriptFileNameMessage, argument.Name))
            .ThrowIf(argument => argument.Value.Any(initScriptContent => initScriptContent.Value == null), argument => new ArgumentException(initScriptContentMessage, argument.Name));
    }

    /// <inheritdoc />
    protected override MongoDbAtlasLocalBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MongoDbAtlasLocalConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MongoDbAtlasLocalBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MongoDbAtlasLocalConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MongoDbAtlasLocalBuilder Merge(MongoDbAtlasLocalConfiguration oldValue, MongoDbAtlasLocalConfiguration newValue)
    {
        return new MongoDbAtlasLocalBuilder(new MongoDbAtlasLocalConfiguration(oldValue, newValue));
    }
}