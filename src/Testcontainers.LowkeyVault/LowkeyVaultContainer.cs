namespace Testcontainers.LowkeyVault;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class LowkeyVaultContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LowkeyVaultContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public LowkeyVaultContainer(LowkeyVaultConfiguration configuration)
        : base(configuration) { }

    /// <summary>
    /// Gets the base HTTPS address for the Lowkey Vault service.
    /// </summary>
    /// <returns>The base address URL.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(
            Uri.UriSchemeHttps,
            Hostname,
            GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultPort)
        ).ToString();
    }

    /// <summary>
    /// Gets the URL used to request the authentication token.
    /// </summary>
    /// <returns>The authentication token URL.</returns>
    public string GetAuthTokenUrl()
    {
        const string identityAuthTokenUriPath = "/metadata/identity/oauth2/token";
        return new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultTokenPort),
            identityAuthTokenUriPath
        ).ToString();
    }

    /// <summary>
    /// Gets the default certificate from the Lowkey Vault service.
    /// </summary>
    /// <returns>A collection containing the default <see cref="X509Certificate2" />.</returns>
    public async Task<X509Certificate2Collection> GetCertificateAsync()
    {
        const string defaultCertFilePathUriPath = "/metadata/default-cert/lowkey-vault.p12";

        var requestUri = new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultTokenPort),
            defaultCertFilePathUriPath
        ).Uri;

        using var httpClient = new HttpClient();

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        using var httpResponseMessage = await httpClient
            .SendAsync(httpRequestMessage)
            .ConfigureAwait(false);

        httpResponseMessage.EnsureSuccessStatusCode();

        var certificateBytes = await httpResponseMessage
            .Content.ReadAsByteArrayAsync()
            .ConfigureAwait(false);

        var certificatePassword = await GetCertificatePasswordAsync().ConfigureAwait(false);

#if NET9_0_OR_GREATER
        return X509CertificateLoader.LoadPkcs12Collection(certificateBytes, certificatePassword);
#else
        var certificate = new X509Certificate2(certificateBytes, certificatePassword);
        return new X509Certificate2Collection(certificate);
#endif
    }

    /// <summary>
    /// Gets the password for the default certificate from the Lowkey Vault service.
    /// </summary>
    /// <returns>The default certificate password.</returns>
    public async Task<string> GetCertificatePasswordAsync()
    {
        const string defaultCertPasswordUriPath = "/metadata/default-cert/password";

        var requestUri = new UriBuilder(
            Uri.UriSchemeHttp,
            Hostname,
            GetMappedPublicPort(LowkeyVaultBuilder.LowkeyVaultTokenPort),
            defaultCertPasswordUriPath
        ).Uri;

        using var httpClient = new HttpClient();

        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);

        using var httpResponseMessage = await httpClient
            .SendAsync(httpRequestMessage)
            .ConfigureAwait(false);

        httpResponseMessage.EnsureSuccessStatusCode();

        return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
    }
}
