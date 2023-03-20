namespace Testcontainers.K3s.Tests;

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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateNamespaceSuccessfully()
    {
        var config = _k3sConainter.GetKubeConfig();

        Assert.NotNull(config);
        Assert.NotEmpty(config);

        var tempFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
        await File.WriteAllTextAsync(tempFile, config);

        var configFile = new FileInfo(tempFile);
        var k8sConfig = await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(configFile);
        var client = new Kubernetes(k8sConfig);

        var res = await client.CoreV1.CreateNamespaceWithHttpMessagesAsync(new V1Namespace
            { Metadata = new V1ObjectMeta { Name = "namespace" } });
        Assert.Equal(System.Net.HttpStatusCode.Created, res.Response.StatusCode);
    }
}