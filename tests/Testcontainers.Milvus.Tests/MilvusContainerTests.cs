using Docker.DotNet.Models;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Commons;
using DotNet.Testcontainers.Containers;
using Milvus.Client;
using Xunit;

namespace Testcontainers.Milvus.Tests;

public class MilvusContainerTests
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WithDefaultConfig()
    {
        var container = new MilvusBuilder()
            .WithImage("milvusdb/milvus:v2.3.10")
            .Build();

        try
        {
            await container.StartAsync();

            var client = new MilvusClient(container.GetEndpoint());
            var version = await client.GetVersionAsync();

            Assert.Equal("v2.3.10", version);
        }
        finally
        {
            await container.StopAsync();
        }
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task WithExternalEtcd()
    {
        var network = new NetworkBuilder().Build();
        var etcdContainer = new ContainerBuilder()
            .WithImage("quay.io/coreos/etcd:v3.5.5")
            .WithNetwork(network)
            .WithNetworkAliases("etcd")
            .WithCommand(
                "etcd",
                "-advertise-client-urls=http://127.0.0.1:2379",
                "-listen-client-urls=http://0.0.0.0:2379",
                "--data-dir=/etcd")
            .WithEnvironment("ETCD_AUTO_COMPACTION_MODE", "revision")
            .WithEnvironment("ETCD_AUTO_COMPACTION_RETENTION", "1000")
            .WithEnvironment("ETCD_QUOTA_BACKEND_BYTES", "4294967296")
            .WithEnvironment("ETCD_SNAPSHOT_COUNT", "50000")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(".*ready to serve client requests.*"))
            .Build();

        var milvusContainer = new MilvusBuilder()
            .WithImage("milvusdb/milvus:v2.3.10")
            .WithNetwork(network)
            .WithEtcdEndpoint("etcd:2379")
            .DependsOn(etcdContainer)
            .Build();

        await milvusContainer.StartAsync();

        try
        {

            var client = new MilvusClient(milvusContainer.GetEndpoint());
            var version = await client.GetVersionAsync();

            Assert.Equal("v2.3.10", version);
        }
        finally
        {
            await milvusContainer.StopAsync();
        }
    }
}
