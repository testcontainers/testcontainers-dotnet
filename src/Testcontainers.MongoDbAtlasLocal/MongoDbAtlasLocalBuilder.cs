namespace Testcontainers.MongoDbAtlasLocal;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MongoDbAtlasLocalBuilder : ContainerBuilder<MongoDbAtlasLocalBuilder, MongoDbAtlasLocalContainer, MongoDbAtlasLocalConfiguration>
{
    public const ushort MongoDbAtlasLocalPort = 27017;

    public const string InitScriptsDirectoryPath = "/docker-entrypoint-initdb.d/";

    private static readonly string[] InitScriptFileExtensions = { ".js", ".sh" };

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDbAtlasLocalBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>mongodb/mongodb-atlas-local:8.0.32</c>).
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
    /// <remarks>
    /// Authentication is disabled by default. Set both the username and the password
    /// to create a root user and enable authentication.
    /// </remarks>
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
    /// <remarks>
    /// Authentication is disabled by default. Set both the username and the password
    /// to create a root user and enable authentication.
    /// </remarks>
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
    /// Defaults to <c>test</c>. The database is not added to the connection string, because MongoDb
    /// would use it as the authentication database, which does not contain the root user.
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
    /// JavaScript (<c>.js</c>) scripts run in <c>mongosh</c> against the init database, shell (<c>.sh</c>)
    /// scripts run in <c>bash</c>. Scripts run in alphabetical order of their file names, before the
    /// container is reported ready, and only once. A restarted or reused container keeps its data and
    /// does not run them again. Init scripts can create Atlas Search indexes.
    /// </remarks>
    /// <param name="scriptFilePath">The host path of the init script.</param>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    /// <exception cref="ArgumentException">Thrown when the file name does not end with <c>.js</c> or <c>.sh</c>.</exception>
    public MongoDbAtlasLocalBuilder WithInitScript(string scriptFilePath)
    {
        _ = Guard.Argument(scriptFilePath, nameof(scriptFilePath))
            .NotNull()
            .NotEmpty();

        ValidateInitScriptFileName(Path.GetFileName(scriptFilePath), nameof(scriptFilePath));

        return WithResourceMapping(FilePath.Of(scriptFilePath), DirectoryPath.Of(InitScriptsDirectoryPath));
    }

    /// <summary>
    /// Copies an init script to the container to seed the deployment on its first start.
    /// </summary>
    /// <remarks>
    /// See <see cref="WithInitScript(string)" /> for how init scripts run. The file name's extension
    /// selects the interpreter, either <c>.js</c> or <c>.sh</c>.
    /// </remarks>
    /// <param name="fileName">The file name of the init script, e.g. <c>01-seed.js</c>.</param>
    /// <param name="scriptContent">The content of the init script.</param>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    /// <exception cref="ArgumentException">Thrown when the file name contains a path separator or does not end with <c>.js</c> or <c>.sh</c>.</exception>
    public MongoDbAtlasLocalBuilder WithInitScriptContent(string fileName, string scriptContent)
    {
        _ = Guard.Argument(fileName, nameof(fileName))
            .NotNull()
            .NotEmpty()
            .ThrowIf(argument => argument.Value.IndexOfAny(new[] { '/', '\\' }) >= 0, argument => new ArgumentException("The init script file name must not contain a path separator.", argument.Name));

        _ = Guard.Argument(scriptContent, nameof(scriptContent))
            .NotNull();

        ValidateInitScriptFileName(fileName, nameof(fileName));

        return WithResourceMapping(Encoding.UTF8.GetBytes(scriptContent), FilePath.Of(InitScriptsDirectoryPath + fileName));
    }

    /// <summary>
    /// Disables the telemetry the MongoDb Atlas Local container sends to MongoDb.
    /// </summary>
    /// <returns>A configured instance of <see cref="MongoDbAtlasLocalBuilder" />.</returns>
    public MongoDbAtlasLocalBuilder WithNoTelemetry()
    {
        return WithEnvironment("DO_NOT_TRACK", "1");
    }

    /// <inheritdoc />
    public override MongoDbAtlasLocalContainer Build()
    {
        Validate();
        return new MongoDbAtlasLocalContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MongoDbAtlasLocalBuilder Init()
    {
        // The runner's healthcheck checks mongod, the replica set, mongot (the Atlas Search
        // process) and that the init scripts have finished. We run it directly instead of
        // waiting for the image's HEALTHCHECK, whose first probe runs 30 seconds after start.
        return base.Init()
            .WithPortBinding(MongoDbAtlasLocalPort, true)
            .WithConnectionStringProvider(new MongoDbAtlasLocalConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("runner", "healthcheck"));
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string message = "Missing username or password. Both must be specified for a user to be created.";

        base.Validate();

        _ = Guard.Argument(DockerResourceConfiguration, "Credentials")
            .ThrowIf(argument => 1.Equals(new[] { argument.Value.Username, argument.Value.Password }.Count(string.IsNullOrWhiteSpace)), argument => new ArgumentException(message, argument.Name));
    }

    /// <summary>
    /// Validates that the runner executes the init script.
    /// </summary>
    /// <remarks>
    /// The runner only executes files ending with <c>.js</c> or <c>.sh</c> (case-sensitive)
    /// and silently skips any other file, e.g. <c>seed.json</c> or <c>seed.JS</c>.
    /// </remarks>
    /// <param name="fileName">The file name of the init script.</param>
    /// <param name="parameterName">The name of the validated parameter.</param>
    private static void ValidateInitScriptFileName(string fileName, string parameterName)
    {
        _ = Guard.Argument(fileName, parameterName)
            .ThrowIf(argument => !InitScriptFileExtensions.Any(fileExtension => argument.Value.EndsWith(fileExtension, StringComparison.Ordinal)), argument => new ArgumentException("The init script file name must end with .js or .sh, the container skips any other file.", argument.Name));
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