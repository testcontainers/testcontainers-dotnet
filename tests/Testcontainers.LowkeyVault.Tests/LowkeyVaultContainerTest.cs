namespace Testcontainers.LowkeyVault;

public sealed class LowkeyVaultContainerTest : IAsyncLifetime
{
    private readonly LowkeyVaultContainer _fakeLowkeyVaultContainer = new LowkeyVaultBuilder().Build();

    public Task InitializeAsync()
    {
        return _fakeLowkeyVaultContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _fakeLowkeyVaultContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task TestContainerDefaults()
    {
        // Given
        const string Alias = "lowkey-vault.local";

        // When
        var tokenEndpoint = _fakeLowkeyVaultContainer.GetTokenEndpointUrl();

        var keyStore = await _fakeLowkeyVaultContainer.GetDefaultKeyStore();

        var password = await _fakeLowkeyVaultContainer.GetDefaultKeyStorePassword();

        // Then
        await VerifyTokenEndpointIsWorking(tokenEndpoint, CreateHttpClientHandlerWithDisabledSslValidation());

        Assert.NotNull(keyStore);
        Assert.NotNull(password);
        Assert.Contains(keyStore, cert => cert.Subject.Split('=')?.LastOrDefault() == Alias);
    }


    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task TestThatSetAndGetSecretWorksWithNoOpCredential()
    {
        await VerifyThatSetAndGetSecretWorks(CreateNoOpCredential());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task TestThatSetAndGetSecretWorksWithManagedIdentity()
    {
        // Set ENV vars to configure the token provider endpoint of the managed identity credential
        Environment.SetEnvironmentVariable("IDENTITY_ENDPOINT", _fakeLowkeyVaultContainer.GetTokenEndpointUrl());
        Environment.SetEnvironmentVariable("IDENTITY_HEADER", "header");

        await VerifyThatSetAndGetSecretWorks(CreateDefaultAzureCredential());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task TestThatCreateAndDownloadCertificateWorksWithNoOpCredential()
    {
        await VerifyThatCreateAndDownloadCertificateWorks(CreateNoOpCredential());
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task TestThatCreateAndDownloadCertificateWorksWithManagedIdentity()
    {
        // Set ENV vars to configure the token provider endpoint of the managed identity credential
        Environment.SetEnvironmentVariable("IDENTITY_ENDPOINT", _fakeLowkeyVaultContainer.GetTokenEndpointUrl());
        Environment.SetEnvironmentVariable("IDENTITY_HEADER", "header");

        await VerifyThatCreateAndDownloadCertificateWorks(CreateDefaultAzureCredential());
    }

    private async Task VerifyThatSetAndGetSecretWorks(TokenCredential credential)
    {
        //Given
        const string SecretName = "name";
        const string SecretValue = "value";

        var vaultUrl = _fakeLowkeyVaultContainer.GetDefaultVaultBaseUrl();

        var secretClient = new SecretClient(new Uri(vaultUrl), credential, CreateSecretClientOption());

        await secretClient.SetSecretAsync(SecretName, SecretValue);

        //When
        var secret = await secretClient.GetSecretAsync(SecretName);

        //Then
        Assert.NotNull(secret.Value);
        Assert.Equal(SecretName, secret.Value.Name);
        Assert.Equal(SecretValue, secret.Value.Value);
    }

    private async Task VerifyThatCreateAndDownloadCertificateWorks(TokenCredential credential)
    {
        //Given
        const string CertificateName = "certificate";
        const string Subject = "CN=example.com";

        var vaultUrl = _fakeLowkeyVaultContainer.GetDefaultVaultBaseUrl();

        var certificateClient = new CertificateClient(new Uri(vaultUrl), credential, CreateCertificateClientOption());

        var certificatePolicy = new CertificatePolicy("Self", Subject)
        {
            KeyType = CertificateKeyType.Rsa,
            KeySize = 2048,
            Exportable = true,
            ContentType = CertificateContentType.Pkcs12,
            ValidityInMonths = 12
        };

        var certOp = await certificateClient.StartCreateCertificateAsync(CertificateName, certificatePolicy);

        await certOp.WaitForCompletionAsync();

        //When
        var response = await certificateClient.DownloadCertificateAsync(CertificateName);

        var certificate = response?.Value;

        //Then
        Assert.Equal(Subject, certificate.Subject);
        Assert.NotNull(certificate.GetRSAPublicKey());
        Assert.NotNull(certificate.GetRSAPrivateKey());
    }

    private static NoopCredentials CreateNoOpCredential()
    {
        return new NoopCredentials();
    }

    private static DefaultAzureCredential CreateDefaultAzureCredential()
    {
        return new DefaultAzureCredential();
    }

    private static SecretClientOptions CreateSecretClientOption()
    {
        return GetClientOptions(new SecretClientOptions(SecretClientOptions.ServiceVersion.V7_4)
        {
            DisableChallengeResourceVerification = true,
            RetryPolicy = new RetryPolicy(0, DelayStrategy.CreateFixedDelayStrategy(TimeSpan.Zero))
        });
    }

    private static CertificateClientOptions CreateCertificateClientOption()
    {
        return GetClientOptions(new CertificateClientOptions(CertificateClientOptions.ServiceVersion.V7_4)
        {
            DisableChallengeResourceVerification = true,
            RetryPolicy = new RetryPolicy(0, DelayStrategy.CreateFixedDelayStrategy(TimeSpan.Zero))
        });
    }

    private static T GetClientOptions<T>(T options) where T : ClientOptions
    {
        DisableSslValidationOnClientOptions(options);
        return options;
    }

    /// <summary>
    /// Disables server certification callback.
    /// <br/>
    /// <b>WARNING: Do not use in production environments.</b>
    /// </summary>
    /// <param name="options"></param>
    private static void DisableSslValidationOnClientOptions(ClientOptions options)
    {
        options.Transport = new HttpClientTransport(CreateHttpClientHandlerWithDisabledSslValidation());
    }

    private static HttpClientHandler CreateHttpClientHandlerWithDisabledSslValidation()
    {
        return new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
    }

    private static async Task VerifyTokenEndpointIsWorking(string endpointUrl, HttpClientHandler httpClientHandler)
    {
        using var httpClient = new HttpClient(httpClientHandler);

        var requestUri = $"{endpointUrl}?resource=https://localhost";
        using var request = new HttpRequestMessage(HttpMethod.Get, requestUri);

        try
        {
            using var response = await httpClient.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        catch (Exception ex)
        {
            Assert.Fail($"Request failed: {ex.Message}");
        }
    }
}

/// <summary>
/// Allows us to bypass authentication when using Lowkey Vault.
/// <br/>
/// <b>WARNING: Will not work with real Azure services.</b>
/// </summary>
internal class NoopCredentials : TokenCredential
{
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new AccessToken("noop", DateTimeOffset.MaxValue);
    }

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return new ValueTask<AccessToken>(GetToken(requestContext, cancellationToken));
    }
}