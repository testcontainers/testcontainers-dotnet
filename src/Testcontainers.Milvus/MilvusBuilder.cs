namespace Testcontainers.Milvus;

[PublicAPI]
public class MilvusBuilder : ContainerBuilder<MilvusBuilder, MilvusContainer, MilvusConfiguration>
{
    public const string MilvusImage = "milvusdb/milvus:v2.3.10";
    public const ushort MilvusGrpcPort = 19530;
    public const ushort MilvusManagementPort = 9091;

    public MilvusBuilder() : this(new MilvusConfiguration())
        => DockerResourceConfiguration = Init().DockerResourceConfiguration;

    private MilvusBuilder(MilvusConfiguration dockerResourceConfiguration) : base(dockerResourceConfiguration)
        => DockerResourceConfiguration = dockerResourceConfiguration;

    public override MilvusContainer Build()
    {
        Validate();
        return new MilvusContainer(DockerResourceConfiguration, TestcontainersSettings.Logger);
    }

    public MilvusBuilder WithEtcdEndpoint(string? etcdEndpoint)
        => WithEnvironment("ETCD_USE_EMBED", "false")
            .WithEnvironment("ETCD_DATA_DIR", "")
            .WithEnvironment("ETCD_CONFIG_PATH", "")
            .WithEnvironment("ETCD_ENDPOINTS", etcdEndpoint);

    protected override MilvusBuilder Init()
    {
        const string etcdYaml = """
                                listen-client-urls: http://0.0.0.0:2379
                                advertise-client-urls: http://0.0.0.0:2379
                                """;

        var builder = base.Init()
            .WithImage(MilvusImage)
            .WithEnvironment("COMMON_STORAGETYPE", "local")
            .WithCommand("milvus", "run", "standalone")
            .WithPortBinding(MilvusGrpcPort, true)
            .WithPortBinding(MilvusManagementPort, true)
            .WithWaitStrategy(
                Wait.ForUnixContainer()
                    .UntilHttpRequestIsSucceeded(h => h
                        .ForPort(MilvusManagementPort)
                        .ForPath("/healthz")));

        // For embedded etcd only; see WithEtcdEndpoint for using an external etcd.
        builder = builder
            .WithEnvironment("ETCD_USE_EMBED", "true")
            .WithEnvironment("ETCD_DATA_DIR", "/var/lib/milvus/etcd")
            .WithEnvironment("ETCD_CONFIG_PATH", "/milvus/configs/embedEtcd.yaml")
            .WithResourceMapping(Encoding.UTF8.GetBytes(etcdYaml), "/milvus/configs/embedEtcd.yaml");

        return builder;
    }

    protected override MilvusBuilder Clone(IResourceConfiguration<CreateContainerParameters> resourceConfiguration)
        => Merge(DockerResourceConfiguration, new MilvusConfiguration(resourceConfiguration));

    protected override MilvusBuilder Merge(MilvusConfiguration oldValue, MilvusConfiguration newValue)
        => new(new MilvusConfiguration(oldValue, newValue));

    protected override MilvusConfiguration DockerResourceConfiguration { get; }

    protected override MilvusBuilder Clone(IContainerConfiguration resourceConfiguration)
        => Merge(DockerResourceConfiguration, new MilvusConfiguration(resourceConfiguration));
}
