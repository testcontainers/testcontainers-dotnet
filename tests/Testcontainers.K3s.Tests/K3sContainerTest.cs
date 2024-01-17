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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateNamespaceReturnsHttpStatusCodeCreated()
    {
        // Given
        using var kubeconfigStream = new MemoryStream();

        var kubeconfig = await _k3sConainter.GetKubeconfigAsync()
            .ConfigureAwait(true);

        await kubeconfigStream.WriteAsync(Encoding.Default.GetBytes(kubeconfig))
            .ConfigureAwait(true);

        var clientConfiguration = await KubernetesClientConfiguration.BuildConfigFromConfigFileAsync(kubeconfigStream)
            .ConfigureAwait(true);

        using var client = new Kubernetes(clientConfiguration);

        // When
        using var response = await client.CoreV1.CreateNamespaceWithHttpMessagesAsync(new V1Namespace(metadata: new V1ObjectMeta(name: Guid.NewGuid().ToString("D"))))
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.Created, response.Response.StatusCode);
    }
}