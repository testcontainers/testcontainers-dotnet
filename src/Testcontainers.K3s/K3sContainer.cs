namespace Testcontainers.K3s;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class K3sContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="K3sContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public K3sContainer(K3sConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Kubeconfig.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Task that completes when the Kubeconfig has been read.</returns>
    public async Task<string> GetKubeconfigAsync(CancellationToken ct = default)
    {
        var kubeconfigBytes = await ReadFileAsync("/etc/rancher/k3s/k3s.yaml", ct)
            .ConfigureAwait(false);

        var kubeconfig = Encoding.Default.GetString(kubeconfigBytes);

        var server = new UriBuilder(Uri.UriSchemeHttps, Hostname, GetMappedPublicPort(K3sBuilder.KubeSecurePort)).ToString();

        return RegexPatterns.KubeconfigServer().Replace(kubeconfig, "server: " + server);
    }
}