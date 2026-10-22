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

    private const string DockerSocketFilePath = "/var/run/docker.sock";

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
        var dockerEndpoint = (DockerResourceConfiguration.DockerEndpointAuthConfig ?? TestcontainersSettings.OS.DockerEndpointAuthConfig)?.Endpoint;
        return WithBindMount(GetRemoteDockerSocketPath(dockerEndpoint), DockerSocketFilePath, AccessMode.ReadWrite);
    }

    /// <inheritdoc />
    public override MiniStackContainer Build()
    {
        Validate();
        return new MiniStackContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override void Validate()
    {
        const string regionMessage = "The region must not be empty.";

        const string credentialsMessage = "The access key and the secret key must both be set.";

        base.Validate();

        var environments = DockerResourceConfiguration.Environments;

        _ = Guard.Argument(DockerResourceConfiguration, "Region")
            .ThrowIf(argument => environments.TryGetValue(RegionEnvVar, out var region) && string.IsNullOrWhiteSpace(region), argument => new ArgumentException(regionMessage, argument.Name));

        _ = Guard.Argument(DockerResourceConfiguration, "Credentials")
            .ThrowIf(argument => new[] { AccessKeyEnvVar, SecretKeyEnvVar }.Any(name => environments.TryGetValue(name, out var value) && string.IsNullOrWhiteSpace(value)), argument => new ArgumentException(credentialsMessage, argument.Name));
    }

    /// <inheritdoc />
    protected override MiniStackBuilder Init()
    {
        var builder = base.Init()
            .WithPortBinding(MiniStackPort, true)
            .WithConnectionStringProvider(() => new MiniStackConnectionStringProvider())
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request =>
                request.ForPath(HealthPath).ForPort(MiniStackPort).ForStatusCode(System.Net.HttpStatusCode.OK)));

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
    /// Resolves the Docker socket path as the Docker daemon sees it, which is the
    /// source of the bind mount.
    /// </summary>
    /// <remarks>
    /// Mirrors Testcontainers for Java: <c>TESTCONTAINERS_DOCKER_SOCKET_OVERRIDE</c>
    /// wins; a Unix socket inside the user's home directory (Docker Desktop,
    /// Rancher Desktop, Colima) is a VM-forwarded socket, so the daemon's own
    /// <c>/var/run/docker.sock</c> is used; any other Unix socket is used as is.
    /// </remarks>
    internal static string GetRemoteDockerSocketPath(Uri dockerEndpoint)
    {
        if (!string.IsNullOrEmpty(TestcontainersSettings.DockerSocketOverride))
        {
            return TestcontainersSettings.DockerSocketOverride;
        }

        if (dockerEndpoint == null || !"unix".Equals(dockerEndpoint.Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return DockerSocketFilePath;
        }

        var socketPath = dockerEndpoint.AbsolutePath;
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        return !string.IsNullOrEmpty(home) && socketPath.StartsWith(home.TrimEnd('/') + "/", StringComparison.Ordinal) ? DockerSocketFilePath : socketPath;
    }
}