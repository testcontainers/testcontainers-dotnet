namespace Testcontainers.K3s;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class K3sContainer : DockerContainer
{
    private string _kubeConfigYaml;

    private void OnContainerStarted() {
        InitKubeConfigAsync().Wait();
    }

    private string GetConfigWithServerUrl(string kubeConfigYaml, string serverUrl) {
        var yaml = new YamlStream();
        using var reader = new StringReader(kubeConfigYaml);
        yaml.Load(reader);
        
        var rootNode = (YamlMappingNode) yaml.Documents[0].RootNode;
        var clusters = (YamlMappingNode) rootNode["clusters"][0];
        ((YamlScalarNode) clusters.Children["cluster"]["server"]).Value = serverUrl;
        
        using var writer = new StringWriter();
        yaml.Save(writer);
        
        return writer.ToString();
    }

    private async Task InitKubeConfigAsync() {
        var configBytes = await ReadFileAsync("/etc/rancher/k3s/k3s.yaml");
        var serverUrl = $"https://{Hostname}:{GetMappedPublicPort(K3sBuilder.KubeSecurePort)}";
        var kubeConfigYaml = Encoding.UTF8.GetString(configBytes);
        _kubeConfigYaml = GetConfigWithServerUrl(kubeConfigYaml, serverUrl);
    }
    
    public string GetKubeConfig() => _kubeConfigYaml;

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