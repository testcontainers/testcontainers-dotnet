namespace Testcontainers.K3s;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class K3sContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="K3sContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    /// <param name="logger">The logger.</param>
    public K3sContainer(K3sConfiguration configuration, ILogger logger)
        : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the Kubeconfig.
    /// </summary>
    /// <returns>Task that completes when the Kubeconfig has been read.</returns>
    public async Task<string> GetKubeconfigAsync()
    {
        var kubeconfigBytes = await ReadFileAsync("/etc/rancher/k3s/k3s.yaml")
            .ConfigureAwait(false);

        var kubeconfig = Encoding.Default.GetString(kubeconfigBytes);

        var server = new UriBuilder(Uri.UriSchemeHttps, Hostname, GetMappedPublicPort(K3sBuilder.KubeSecurePort)).ToString();

        return Regex.Replace(kubeconfig, "server:\\s?[:/\\.\\d\\w]+", "server: " + server, RegexOptions.None, TimeSpan.FromSeconds(1));
    }
}