namespace Testcontainers.LowkeyVault;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class LowkeyVaultContainer : DockerContainer
{
    /// <summary>
    /// Gets a configured HTTP client
    /// </summary>
    private static HttpClient HttpClient => new();

    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public LowkeyVaultContainer(LowkeyVaultConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the URL of the default vault.
    /// </summary>
    /// <returns>The default vault base URL.</returns>
    public string GetDefaultVaultBaseUrl()
    {
        return new UriBuilder(Uri.UriSchemeHttps, Hostname, GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultPort)).ToString();
    }

    /// <summary>
    /// Gets the full URL of the token endpoint.
    /// </summary>
    /// <returns>The full token endpoint URL.</returns>
    public string GetTokenEndpointUrl()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultTokenPort), LowkeyVaultBuilder.TokenEndPointPath).ToString();
    }

    /// <summary>
    /// Gets a <see cref="X509Certificate2Collection"/> containing the default certificate shipped with Lowkey Vault.
    /// </summary>
    /// <returns>The <see cref="X509Certificate2Collection"/>.</returns>
    public async Task<X509Certificate2Collection> GetDefaultKeyStore()
    {
        var requestUri = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultTokenPort), "/metadata/default-cert/lowkey-vault.p12").ToString();
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        try
        {
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var keyStoreBytes = await response.Content.ReadAsByteArrayAsync();

            var password = await GetDefaultKeyStorePassword();

            // Load the PKCS12 keystore
#if NET9_0_OR_GREATER
            return X509CertificateLoader.LoadPkcs12Collection(keyStoreBytes, password, X509KeyStorageFlags.DefaultKeySet);
#else
            return [new X509Certificate2(keyStoreBytes, password, X509KeyStorageFlags.DefaultKeySet)];
#endif
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to get default key store", e);
        }
    }

    /// <summary>
    /// Gets the password protecting the default certificate shipped with Lowkey Vault.
    /// </summary>
    /// <returns>The password.</returns>
    public async Task<string> GetDefaultKeyStorePassword()
    {
        var requestUri = new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultTokenPort), "/metadata/default-cert/password").ToString();
        var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        try
        {
            var response = await HttpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException("Failed to get default key store password", e);
        }
    }
}