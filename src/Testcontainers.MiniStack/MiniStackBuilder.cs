namespace Testcontainers.MiniStack;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
/// <remarks>
/// MiniStack is a free, open-source AWS emulator that serves all its services on a single port.
/// Use <see cref="MiniStackContainer.GetConnectionString" /> as the AWS SDK service URL.
/// </remarks>
[PublicAPI]
public sealed class MiniStackBuilder : ContainerBuilder<MiniStackBuilder, MiniStackContainer, MiniStackConfiguration>
{
    /// <summary>
    /// The MiniStack gateway port that serves every AWS service.
    /// </summary>
    public const ushort MiniStackPort = 4566;

    /// <summary>
    /// The health endpoint the wait strategy polls for HTTP 200.
    /// </summary>
    public const string HealthPath = "/_ministack/health";

    /// <summary>
    /// The AWS access key ID MiniStack accepts by default.
    /// </summary>
    public const string AccessKey = "test";

    /// <summary>
    /// The AWS secret access key MiniStack accepts by default.
    /// </summary>
    public const string SecretKey = "test";

    /// <summary>
    /// The AWS region MiniStack reports by default.
    /// </summary>
    public const string Region = "us-east-1";

    internal const string RegionEnvVar = "MINISTACK_REGION";

    internal const string AccessKeyEnvVar = "AWS_ACCESS_KEY_ID";

    internal const string SecretKeyEnvVar = "AWS_SECRET_ACCESS_KEY";

    internal const string ImagePrefixEnvVar = "MINISTACK_IMAGE_PREFIX";

    internal const string ServicesEnvVar = "SERVICES";

    /// <summary>
    /// The readiness endpoint, which reports HTTP 200 once the ready scripts have run.
    /// </summary>
    public const string ReadyPath = "/_ministack/ready";

    private const string DockerSocketFilePath = "/var/run/docker.sock";

    private const string InitScriptsDirectoryPath = "/docker-entrypoint-initaws.d/";

    private const string ReadyScriptsDirectoryPath = "/docker-entrypoint-initaws.d/ready.d/";

    private static readonly string[] ScriptFileExtensions = { ".sh", ".py" };

    private static readonly char[] PathSeparators = { '/', '\\' };

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// The full Docker image name, including the image repository and tag
    /// (e.g., <c>ministackorg/ministack:1.5.31</c>).
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ministackorg/ministack/tags" />.
    /// </remarks>
    public MiniStackBuilder(string image)
        : this(new DockerImage(image))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackBuilder" /> class.
    /// </summary>
    /// <param name="image">
    /// An <see cref="IImage" /> instance that specifies the Docker image to be used
    /// for the container builder configuration.
    /// </param>
    /// <remarks>
    /// Docker image tags available at <see href="https://hub.docker.com/r/ministackorg/ministack/tags" />.
    /// </remarks>
    public MiniStackBuilder(IImage image)
        : this(new MiniStackConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiniStackBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MiniStackBuilder(MiniStackConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MiniStackConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Overrides the AWS region the emulator reports to clients.
    /// </summary>
    /// <remarks>
    /// Sets <c>MINISTACK_REGION</c>, the server-side default region used for
    /// generated ARNs. AWS SDK clients still need their own region configured;
    /// use <see cref="MiniStackContainer.GetRegion" /> for that.
    /// </remarks>
    /// <param name="region">The AWS region, e.g. <c>eu-west-1</c>.</param>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithRegion(string region)
    {
        return WithEnvironment(RegionEnvVar, region);
    }

    /// <summary>
    /// Overrides the AWS credentials the emulator accepts.
    /// </summary>
    /// <remarks>
    /// MiniStack derives the account ID from the access key (a 12-digit key is
    /// treated as the account ID for multi-tenant isolation), so custom
    /// credentials let a test exercise non-default-account behaviour.
    /// </remarks>
    /// <param name="accessKey">The AWS access key ID.</param>
    /// <param name="secretKey">The AWS secret access key.</param>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithCredentials(string accessKey, string secretKey)
    {
        return WithEnvironment(AccessKeyEnvVar, accessKey)
            .WithEnvironment(SecretKeyEnvVar, secretKey);
    }

    /// <summary>
    /// Enables disk persistence so state survives a container restart.
    /// </summary>
    /// <remarks>
    /// Sets <c>PERSIST_STATE=1</c> (service state written to <c>STATE_DIR</c> on
    /// shutdown and reloaded on boot) and <c>S3_PERSIST=1</c> (S3 object bytes
    /// written to disk). Mount a volume at <c>/var/lib/ministack</c> as well if
    /// the data has to outlive the container itself.
    /// </remarks>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithPersistence()
    {
        return WithEnvironment("PERSIST_STATE", "1")
            .WithEnvironment("S3_PERSIST", "1");
    }

    /// <summary>
    /// Requires IMDSv2 (token-based) for instance-metadata calls.
    /// </summary>
    /// <remarks>
    /// Sets <c>MINISTACK_IMDS_V2_REQUIRED=1</c>. IMDS GET requests without a valid
    /// <c>X-aws-ec2-metadata-token</c> header are then rejected with HTTP 401.
    /// </remarks>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithImdsV2Required()
    {
        return WithEnvironment("MINISTACK_IMDS_V2_REQUIRED", "1");
    }

    /// <summary>
    /// Activates real infrastructure mode by bind-mounting the host Docker socket.
    /// </summary>
    /// <remarks>
    /// RDS spins up real Postgres/MySQL containers, ElastiCache real Redis, ECS
    /// real Docker containers.
    /// <para>
    /// <b>Security warning:</b> anything running inside MiniStack, including
    /// Lambda handlers and RDS init scripts, gains root-equivalent control of the
    /// host's container engine. Use only on trusted developer machines or
    /// isolated CI runners.
    /// </para>
    /// </remarks>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithRealInfrastructure()
    {
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(realInfrastructure: true));
    }

    /// <summary>
    /// Limits MiniStack to the given AWS services.
    /// </summary>
    /// <remarks>
    /// Sets <c>SERVICES</c> to a comma-separated list, e.g. <c>s3,sqs,dynamodb</c>.
    /// MiniStack does not load the other services, and logs a warning for unknown names.
    /// </remarks>
    /// <param name="services">The AWS service names.</param>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithServices(params string[] services)
    {
        return WithEnvironment(ServicesEnvVar, string.Join(",", services ?? Array.Empty<string>()));
    }

    /// <summary>
    /// Makes RDS report <c>localhost</c> and the published host port as the database endpoint.
    /// </summary>
    /// <remarks>
    /// Sets <c>MINISTACK_RDS_PUBLIC_ENDPOINT=1</c>. Without it, RDS in real infrastructure
    /// mode reports the database container's address on the Docker network, which a test
    /// running on a macOS or Windows host cannot reach.
    /// </remarks>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithRdsPublicEndpoint()
    {
        return WithEnvironment("MINISTACK_RDS_PUBLIC_ENDPOINT", "1");
    }

    /// <summary>
    /// Copies a boot script to the container.
    /// </summary>
    /// <remarks>
    /// Shell (<c>.sh</c>) and Python (<c>.py</c>) scripts run in alphabetical order of their
    /// file names before MiniStack starts serving requests, so they cannot call the AWS APIs.
    /// Use <see cref="WithReadyScript(string)" /> to create AWS resources.
    /// </remarks>
    /// <param name="scriptFilePath">The host file path of the boot script.</param>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithInitScript(string scriptFilePath)
    {
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(initScriptFilePaths: new[] { scriptFilePath }));
    }

    /// <summary>
    /// Creates a boot script in the container.
    /// </summary>
    /// <remarks>
    /// See <see cref="WithInitScript(string)" /> for how the boot scripts run.
    /// </remarks>
    /// <param name="fileName">The file name of the boot script (e.g., <c>01-prepare.sh</c>).</param>
    /// <param name="scriptContent">The content of the boot script.</param>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithInitScriptContent(string fileName, string scriptContent)
    {
        var initScriptContents = new[] { new KeyValuePair<string, string>(fileName, scriptContent) };
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(initScriptContents: initScriptContents));
    }

    /// <summary>
    /// Copies a ready script to the container to seed AWS resources.
    /// </summary>
    /// <remarks>
    /// Shell (<c>.sh</c>) and Python (<c>.py</c>) scripts run in alphabetical order of their
    /// file names once MiniStack serves requests. The AWS CLI (<c>aws</c>, <c>awslocal</c>)
    /// and boto3 are preconfigured with MiniStack's endpoint, region and credentials. The
    /// container is ready after all ready scripts have run, and the start fails if one of
    /// them fails.
    /// </remarks>
    /// <param name="scriptFilePath">The host file path of the ready script.</param>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithReadyScript(string scriptFilePath)
    {
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(readyScriptFilePaths: new[] { scriptFilePath }));
    }

    /// <summary>
    /// Creates a ready script in the container to seed AWS resources.
    /// </summary>
    /// <remarks>
    /// See <see cref="WithReadyScript(string)" /> for how the ready scripts run.
    /// </remarks>
    /// <param name="fileName">The file name of the ready script (e.g., <c>01-seed.sh</c>).</param>
    /// <param name="scriptContent">The content of the ready script.</param>
    /// <returns>A configured instance of <see cref="MiniStackBuilder" />.</returns>
    public MiniStackBuilder WithReadyScriptContent(string fileName, string scriptContent)
    {
        var readyScriptContents = new[] { new KeyValuePair<string, string>(fileName, scriptContent) };
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(readyScriptContents: readyScriptContents));
    }

    /// <inheritdoc />
    public override MiniStackContainer Build()
    {
        Validate();

        // The scripts are mapped after the validation. Creating the resource mappings
        // in the builder methods would throw for invalid arguments before Validate()
        // can report them.
        var miniStackBuilder = this;

        miniStackBuilder = DockerResourceConfiguration.InitScriptFilePaths
            .Aggregate(miniStackBuilder, (builder, scriptFilePath) =>
                builder.WithResourceMapping(FilePath.Of(scriptFilePath), DirectoryPath.Of(InitScriptsDirectoryPath)));

        miniStackBuilder = DockerResourceConfiguration.InitScriptContents
            .Aggregate(miniStackBuilder, (builder, scriptContent) =>
                builder.WithResourceMapping(Encoding.UTF8.GetBytes(scriptContent.Value), FilePath.Of(InitScriptsDirectoryPath + scriptContent.Key)));

        miniStackBuilder = DockerResourceConfiguration.ReadyScriptFilePaths
            .Aggregate(miniStackBuilder, (builder, scriptFilePath) =>
                builder.WithResourceMapping(FilePath.Of(scriptFilePath), DirectoryPath.Of(ReadyScriptsDirectoryPath)));

        miniStackBuilder = DockerResourceConfiguration.ReadyScriptContents
            .Aggregate(miniStackBuilder, (builder, scriptContent) =>
                builder.WithResourceMapping(Encoding.UTF8.GetBytes(scriptContent.Value), FilePath.Of(ReadyScriptsDirectoryPath + scriptContent.Key)));

        // The socket is mounted here, so that it follows the final Docker endpoint even
        // when WithDockerEndpoint(...) is called after WithRealInfrastructure().
        if (true.Equals(DockerResourceConfiguration.RealInfrastructure))
        {
            miniStackBuilder = miniStackBuilder.WithBindMount(GetDockerSocketPath(DockerResourceConfiguration.DockerEndpointAuthConfig.Endpoint), DockerSocketFilePath, AccessMode.ReadWrite);
        }

        return new MiniStackContainer(miniStackBuilder.DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string regionMessage = "The region must not be empty.";

        const string credentialsMessage = "The access key and the secret key must both be set.";

        const string servicesMessage = "The services must contain at least one service name.";

        const string scriptFileNameMessage = "The script file name must end with .sh or .py and cannot contain a path separator.";

        const string scriptContentMessage = "The script content cannot be null.";

        const string duplicateScriptMessage = "Two scripts of the same kind cannot have the same file name.";

        base.Validate();

        Predicate<string> isInvalidScriptFileName = value =>
            string.IsNullOrEmpty(value) || value.IndexOfAny(PathSeparators) >= 0 || !ScriptFileExtensions.Any(fileExtension => value.EndsWith(fileExtension, StringComparison.Ordinal));

        var environments = DockerResourceConfiguration.Environments;

        _ = Guard.Argument(DockerResourceConfiguration, "Region")
            .ThrowIf(argument => environments.TryGetValue(RegionEnvVar, out var region) && string.IsNullOrWhiteSpace(region), argument => new ArgumentException(regionMessage, argument.Name));

        _ = Guard.Argument(DockerResourceConfiguration, "Credentials")
            .ThrowIf(argument => new[] { AccessKeyEnvVar, SecretKeyEnvVar }.Any(name => environments.TryGetValue(name, out var value) && string.IsNullOrWhiteSpace(value)), argument => new ArgumentException(credentialsMessage, argument.Name));

        _ = Guard.Argument(DockerResourceConfiguration, "Services")
            .ThrowIf(argument => environments.TryGetValue(ServicesEnvVar, out var services) && services.Split(',').All(string.IsNullOrWhiteSpace), argument => new ArgumentException(servicesMessage, argument.Name));

        foreach (var scriptFilePaths in new[] { DockerResourceConfiguration.InitScriptFilePaths, DockerResourceConfiguration.ReadyScriptFilePaths })
        {
            _ = Guard.Argument(scriptFilePaths, "ScriptFilePaths")
                .ThrowIf(argument => argument.Value.Any(scriptFilePath => isInvalidScriptFileName(Path.GetFileName(scriptFilePath))), argument => new ArgumentException(scriptFileNameMessage, argument.Name));
        }

        foreach (var scriptContents in new[] { DockerResourceConfiguration.InitScriptContents, DockerResourceConfiguration.ReadyScriptContents })
        {
            _ = Guard.Argument(scriptContents, "ScriptContents")
                .ThrowIf(argument => argument.Value.Any(scriptContent => isInvalidScriptFileName(scriptContent.Key)), argument => new ArgumentException(scriptFileNameMessage, argument.Name))
                .ThrowIf(argument => argument.Value.Any(scriptContent => scriptContent.Value == null), argument => new ArgumentException(scriptContentMessage, argument.Name));
        }

        Func<IEnumerable<string>, IEnumerable<KeyValuePair<string, string>>, bool> hasDuplicateFileName = (scriptFilePaths, scriptContents) =>
            scriptFilePaths.Select(Path.GetFileName).Concat(scriptContents.Select(scriptContent => scriptContent.Key)).GroupBy(fileName => fileName, StringComparer.Ordinal).Any(group => group.Count() > 1);

        _ = Guard.Argument(DockerResourceConfiguration, "InitScripts")
            .ThrowIf(argument => hasDuplicateFileName(argument.Value.InitScriptFilePaths, argument.Value.InitScriptContents), argument => new ArgumentException(duplicateScriptMessage, argument.Name));

        _ = Guard.Argument(DockerResourceConfiguration, "ReadyScripts")
            .ThrowIf(argument => hasDuplicateFileName(argument.Value.ReadyScriptFilePaths, argument.Value.ReadyScriptContents), argument => new ArgumentException(duplicateScriptMessage, argument.Name));
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Init()
    {
        var builder = base.Init()
            .WithPortBinding(MiniStackPort, true)
            .WithConnectionStringProvider(() => new MiniStackConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(request => request.ForPath(HealthPath).ForPort(MiniStackPort))
                // The readiness endpoint returns HTTP 503 until the ready scripts have run.
                // Images that predate it return HTTP 404 and are ready once healthy.
                .UntilHttpRequestIsSucceeded(request => request.ForPath(ReadyPath).ForPort(MiniStackPort)
                    .ForStatusCodeMatching(statusCode => HttpStatusCode.OK.Equals(statusCode) || HttpStatusCode.NotFound.Equals(statusCode))));

        // Forward the hub image name prefix so nested real-infrastructure images
        // (RDS, ElastiCache, EKS, Lambda runtimes) pull through the same private
        // registry as the MiniStack image itself.
        var prefix = TestcontainersSettings.HubImageNamePrefix;
        return string.IsNullOrEmpty(prefix) ? builder : builder.WithEnvironment(ImagePrefixEnvVar, prefix);
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MiniStackConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Merge(MiniStackConfiguration oldValue, MiniStackConfiguration newValue)
    {
        return new MiniStackBuilder(new MiniStackConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// Resolves the source of the Docker socket bind mount the same way the Resource
    /// Reaper does: the <c>TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE</c> setting if set,
    /// otherwise the path of a Unix socket endpoint, otherwise <c>/var/run/docker.sock</c>.
    /// </summary>
    /// <param name="dockerEndpoint">The Docker endpoint.</param>
    /// <returns>The Docker socket path on the Docker host.</returns>
    private static string GetDockerSocketPath(Uri dockerEndpoint)
    {
        if (!string.IsNullOrEmpty(TestcontainersSettings.DockerSocketOverride))
        {
            return TestcontainersSettings.DockerSocketOverride;
        }

        return "unix".Equals(dockerEndpoint?.Scheme, StringComparison.OrdinalIgnoreCase) ? dockerEndpoint.AbsolutePath : DockerSocketFilePath;
    }
}