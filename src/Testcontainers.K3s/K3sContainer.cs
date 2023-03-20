namespace Testcontainers.K3s;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class K3sContainer : DockerContainer
{
    public string KubeConfigYaml { get; private set; }

    private void OnContainerStarted() {
        InitKubeConfig().Wait();
    }

    private async Task InitKubeConfig() {
        var configBytes = await ReadFileAsync("/etc/rancher/k3s/k3s.yaml");
        KubeConfigYaml = Encoding.UTF8.GetString(configBytes);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="K3sContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public K3sContainer(K3sConfiguration configuration, ILogger logger) : base(configuration, logger)
    {
        Started += (_, _) => OnContainerStarted();
    }
}