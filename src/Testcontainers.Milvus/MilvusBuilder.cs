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
    public MilvusBuilder()
        : this(new MilvusConfiguration())
    {
        DockerResourceConfiguration = Init().DockerResourceConfiguration;
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
        // Imitate what is done in the official script
        // https://github.com/milvus-io/milvus/blob/2134f83aa34bfbcc3750d69c4874adca5dd85d62/scripts/standalone_embed.sh#L43

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
            .WithCreateParameterModifier(p =>
            {
                p.Healthcheck = new HealthcheckConfig
                {
                    Test = ["CMD-SHELL", $"curl -f http://localhost:{MilvusManagementPort}/healthz"],
                    Interval = TimeSpan.FromSeconds(30),
                    StartPeriod = 90 * 1_000_000_000L, // 90s
                    Timeout = TimeSpan.FromSeconds(20),
                    Retries = 3,
                };
            })
            .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy());
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
}