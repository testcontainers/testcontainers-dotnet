namespace Testcontainers.K3s;

public sealed class K3sContainerTest : IAsyncLifetime
{
    private readonly K3sContainer _k3sConainter = new K3sBuilder().Build();

    public Task InitializeAsync()
    {
        return _k3sConainter.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _k3sConainter.DisposeAsync().AsTask();
    }

    // Failed to load kernel module br_netfilter with modprobe
    // Failed to load kernel module iptable_nat with modprobe
    // Failed to load kernel module iptable_filter with modprobe
    // Failed to set sysctl: open /proc/sys/net/bridge/bridge-nf-call-iptables: no such file or directory
    // Failed to set sysctl: open /proc/sys/net/netfilter/nf_conntrack_max: permission denied
    // Failed to ApplyOOMScoreAdj" err="write /proc/self/oom_score_adj: permission denied
    // Failed to set rlimit on max file handles" err="operation not permitted
    // Failed to get the info of the filesystem with mountpoint" err="unable to find data in memory cache" mountpoint="/var/lib/rancher/k3s/agent/containerd/io.containerd.snapshotter.v1.overlayfs
    // Failed to start ContainerManager" err="[open /proc/sys/vm/overcommit_memory: permission denied, open /proc/sys/kernel/panic: permission denied, open /proc/sys/kernel/panic_on_oops: permission denied]
    [Fact(Skip = "Container does not start running on Podman")]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateNamespaceReturnsHttpStatusCodeCreated()
    {
        // Given
        using var kubeconfigStream = new MemoryStream();

        var kubeconfig = await _k3sConainter.GetKubeconfigAsync()
            .ConfigureAwait(false);

        await kubeconfigStream.WriteAsync(Encoding.Default.GetBytes(kubeconfig))
            .ConfigureAwait(false);

        var clientConfiguration = await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(kubeconfigStream)
            .ConfigureAwait(false);

        using var client = new Kubernetes(clientConfiguration);

        // When
        using var response = await client.CoreV1.CreateNamespaceWithHttpMessagesAsync(new V1Namespace(metadata: new V1ObjectMeta(name: Guid.NewGuid().ToString("D"))))
            .ConfigureAwait(false);

        // Then
        Assert.Equal(HttpStatusCode.Created, response.Response.StatusCode);
    }
}