namespace Testcontainers.Milvus;

[PublicAPI]
public class MilvusBuilder : ContainerBuilder<MilvusBuilder, MilvusContainer, MilvusConfiguration>
{
    public const string MilvusImage = "milvusdb/milvus:v2.3.10";
    public const ushort MilvusGrpcPort = 19530;
    public const ushort MilvusManagementPort = 9091;

    private string? _etcdEndpoint;

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
    {
        _etcdEndpoint = etcdEndpoint;
        return this;
    }

    protected override MilvusBuilder Init()
    {
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

        if (_etcdEndpoint is null)
        {
            const string etcdYaml = """
                                    listen-client-urls: http://0.0.0.0:2379
                                    advertise-client-urls: http://0.0.0.0:2379
                                    """;

            builder = builder
                .WithResourceMapping(Encoding.UTF8.GetBytes(etcdYaml), "/milvus/configs/embedEtcd.yaml")
                .WithEnvironment("ETCD_USE_EMBED", "true")
                .WithEnvironment("ETCD_DATA_DIR", "/var/lib/milvus/etcd")
                .WithEnvironment("ETCD_CONFIG_PATH", "/milvus/configs/embedEtcd.yaml");
        }
        else
        {
            throw new NotImplementedException();
        }

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
