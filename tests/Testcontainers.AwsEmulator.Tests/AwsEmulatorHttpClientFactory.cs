namespace Testcontainers.AwsEmulator;

// # --8<-- [start:ConfigureAwsEmulatorHttpClientFactory]
public sealed class AwsEmulatorHttpClientFactory : HttpClientFactory, IDisposable
{
    private readonly SocketsHttpHandler _httpMessageHandler = new SocketsHttpHandler();

    public AwsEmulatorHttpClientFactory(X509Certificate2 certificate)
    {
        // Trust only the self-signed certificate of the emulator instance.
        _httpMessageHandler.SslOptions.CertificateChainPolicy = new X509ChainPolicy();
        _httpMessageHandler.SslOptions.CertificateChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        _httpMessageHandler.SslOptions.CertificateChainPolicy.CustomTrustStore.Add(certificate);
        _httpMessageHandler.SslOptions.CertificateChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
    }

    public override HttpClient CreateHttpClient(IClientConfig clientConfig)
    {
        return new HttpClient(_httpMessageHandler, false);
    }

    public void Dispose()
    {
        _httpMessageHandler.Dispose();
    }
}
// # --8<-- [end:ConfigureAwsEmulatorHttpClientFactory]