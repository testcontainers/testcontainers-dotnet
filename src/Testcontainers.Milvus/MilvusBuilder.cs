using DotNet.Testcontainers.Images;

namespace Testcontainers.Milvus;

/// <inheritdoc cref="ContainerBuilder{TBuilderEntity, TContainerEntity, TConfigurationEntity}" />
[PublicAPI]
public sealed class MilvusBuilder : ContainerBuilder<MilvusBuilder, MilvusContainer, MilvusConfiguration>
{
    public const string MilvusEtcdConfigFilePath = "/milvus/configs/embedEtcd.yaml";

    public const string MilvusImage = "milvusdb/milvus:v2.3.10";

    public const ushort MilvusManagementPort = 9091;

    public const ushort MilvusGrpcPort = 19530;

    private static readonly byte[] EtcdConfig = Encoding.Default.GetBytes(string.Join("\n", "advertise-client-urls: http://0.0.0.0:2379", "listen-client-urls: http://0.0.0.0:2379"));

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusBuilder" /> class.
    /// </summary>
    [Obsolete("Use constructor with image as a parameter instead.")]
    public MilvusBuilder()
        : this(new MilvusConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(MilvusImage).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusBuilder" /> class.
    /// </summary>
    /// <param name="image">Docker image tag. Available tags can be found here: <see href="https://hub.docker.com/r/milvusdb/milvus/tags">https://hub.docker.com/r/milvusdb/milvus/tags</see>.</param>
    public MilvusBuilder(string image)
        : this(new MilvusConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusBuilder" /> class.
    /// </summary>
    /// <param name="image">Image instance to use in configuration.</param>
    public MilvusBuilder(IImage image)
        : this(new MilvusConfiguration())
    {
        DockerResourceConfiguration = Init().WithImage(image).DockerResourceConfiguration;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MilvusBuilder" /> class.
    /// </summary>
    /// <param name="resourceConfiguration">The Docker resource configuration.</param>
    private MilvusBuilder(MilvusConfiguration resourceConfiguration)
        : base(resourceConfiguration)
    {
        DockerResourceConfiguration = resourceConfiguration;
    }

    /// <inheritdoc />
    protected override MilvusConfiguration DockerResourceConfiguration { get; }

    /// <summary>
    /// Sets the etcd endpoint.
    /// </summary>
    /// <param name="etcdEndpoint">The etcd endpoint.</param>
    /// <returns>A configured instance of <see cref="MilvusBuilder" />.</returns>
    public MilvusBuilder WithEtcdEndpoint(string etcdEndpoint)
    {
        return WithEnvironment("ETCD_USE_EMBED", "false")
            .WithEnvironment("ETCD_CONFIG_PATH", string.Empty)
            .WithEnvironment("ETCD_DATA_DIR", string.Empty)
            .WithEnvironment("ETCD_ENDPOINTS", etcdEndpoint);
    }

    /// <inheritdoc />
    public override MilvusContainer Build()
    {
        Validate();
        return new MilvusContainer(DockerResourceConfiguration);
    }

    /// <inheritdoc />
    protected override MilvusBuilder Init()
    {
        return base.Init()
            .WithImage(MilvusImage)
            .WithPortBinding(MilvusManagementPort, true)
            .WithPortBinding(MilvusGrpcPort, true)
            .WithCommand("milvus", "run", "standalone")
            .WithEnvironment("DEPLOY_MODE", "STANDALONE")
            .WithEnvironment("COMMON_STORAGETYPE", "local")
            // For embedded etcd only; see WithEtcdEndpoint(string) for using an external etcd.
            .WithEnvironment("ETCD_USE_EMBED", "true")
            .WithEnvironment("ETCD_CONFIG_PATH", MilvusEtcdConfigFilePath)
            .WithEnvironment("ETCD_DATA_DIR", "/var/lib/milvus/etcd")
            .WithResourceMapping(EtcdConfig, MilvusEtcdConfigFilePath)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
            .WithCreateParameterModifier(parameterModifier =>
                parameterModifier.Healthcheck = Healthcheck.Instance);
    }

    /// <inheritdoc />
    protected override MilvusBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MilvusConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MilvusBuilder Clone(IContainerConfiguration resourceConfiguration)
    {
        return Merge(DockerResourceConfiguration, new MilvusConfiguration(resourceConfiguration));
    }

    /// <inheritdoc />
    protected override MilvusBuilder Merge(MilvusConfiguration oldValue, MilvusConfiguration newValue)
    {
        return new MilvusBuilder(new MilvusConfiguration(oldValue, newValue));
    }

    /// <summary>
    /// This setup mirrors the behavior of Milvus's official configuration:
    /// https://github.com/milvus-io/milvus/blob/4def0255a928287f982f1d6b8c53ed32127bb84d/scripts/standalone_embed.sh#L56-L60
    /// </summary>
    private sealed class Healthcheck : HealthcheckConfig
    {
        private Healthcheck()
        {
            const long ninetySeconds = 90 * 1_000_000_000L;
            Test = ["CMD-SHELL", $"curl -f http://localhost:{MilvusManagementPort}/healthz"];
            Interval = TimeSpan.FromSeconds(30);
            Timeout = TimeSpan.FromSeconds(20);
            StartPeriod = ninetySeconds;
            Retries = 3;
        }

        /// <summary>
        /// Gets the <see cref="HealthcheckConfig" /> instance.
        /// </summary>
        public static HealthcheckConfig Instance { get; }
            = new Healthcheck();
    }
}