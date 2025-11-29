namespace Testcontainers.LowkeyVault;

public abstract class LowkeyVaultContainerTest : IAsyncLifetime
{
    private readonly LowkeyVaultContainer _lowkeyVaultContainer = new LowkeyVaultBuilder(TestSession.GetImageFromDockerfile()).Build();

    protected abstract TokenCredential GetTokenCredential();

    public async ValueTask InitializeAsync()
    {
        await _lowkeyVaultContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ServerCertificateValidationSucceedsWithTrustedCertificate()
    {
        // Given
        var baseAddress = _lowkeyVaultContainer.GetBaseAddress();

        var certificates = await _lowkeyVaultContainer.GetCertificateAsync();

        using var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = (_, cert, _, _) => certificates.IndexOf(cert) > -1;

        using var httpClient = new HttpClient(httpMessageHandler);
        httpClient.BaseAddress = new Uri(baseAddress);

        // When
        using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "management/vault");

        using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task GetSecretReturnsSetSecret()
    {
        // Given
        const string secretName = "name";

        const string secretValue = "value";

        var baseAddress = _lowkeyVaultContainer.GetBaseAddress();

        var secretClient = new SecretClient(new Uri(baseAddress), GetTokenCredential(), GetSecretClientOptions());

        await secretClient.SetSecretAsync(secretName, secretValue, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // When
        var keyVaultSecret = await secretClient.GetSecretAsync(secretName, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.NotNull(keyVaultSecret.Value);
        Assert.Equal(secretName, keyVaultSecret.Value.Name);
        Assert.Equal(secretValue, keyVaultSecret.Value.Value);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task DownloadCertificateReturnsCreatedCertificate()
    {
        // Given
        const string certificateName = "certificate";

        const string subject = "CN=localhost";

        var baseAddress = _lowkeyVaultContainer.GetBaseAddress();

        var certificateClient = new CertificateClient(new Uri(baseAddress), GetTokenCredential(), GetCertificateClientOptions());

        var certificatePolicy = new CertificatePolicy("self", subject);
        certificatePolicy.KeyType = CertificateKeyType.Rsa;
        certificatePolicy.KeySize = 2048;
        certificatePolicy.ContentType = CertificateContentType.Pem;
        certificatePolicy.Exportable = true;
        certificatePolicy.ValidityInMonths = 12;

        // When
        var certificateOperation = await certificateClient.StartCreateCertificateAsync(certificateName, certificatePolicy, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        await certificateOperation.WaitForCompletionAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var response = await certificateClient.DownloadCertificateAsync(certificateName, cancellationToken: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var certificate = response!.Value;

        // Then
        Assert.Equal(subject, certificate.Subject);
        Assert.NotNull(certificate.GetRSAPublicKey());
        Assert.NotNull(certificate.GetRSAPrivateKey());
    }

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _lowkeyVaultContainer.DisposeAsync();
    }

    private static SecretClientOptions GetSecretClientOptions()
    {
        var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        var secretClientOptions = new SecretClientOptions();
        secretClientOptions.Transport = new HttpClientTransport(httpMessageHandler);
        secretClientOptions.DisableChallengeResourceVerification = true;
        return secretClientOptions;
    }

    private static CertificateClientOptions GetCertificateClientOptions()
    {
        var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

        var secretClientOptions = new CertificateClientOptions();
        secretClientOptions.Transport = new HttpClientTransport(httpMessageHandler);
        secretClientOptions.DisableChallengeResourceVerification = true;
        return secretClientOptions;
    }

    [UsedImplicitly]
    public sealed class AzureCredentialConfiguration : LowkeyVaultContainerTest
    {
        protected override TokenCredential GetTokenCredential()
        {
            // This isn't a recommended approach. It stops you from running multiple containers
            // at the same time.
            const EnvironmentVariableTarget envVarTarget = EnvironmentVariableTarget.Process;
            Environment.SetEnvironmentVariable("IDENTITY_ENDPOINT", _lowkeyVaultContainer.GetAuthTokenUrl(), envVarTarget);
            Environment.SetEnvironmentVariable("IDENTITY_HEADER", "header", envVarTarget);
            return new DefaultAzureCredential();
        }
    }

    [UsedImplicitly]
    public sealed class NoopCredentialConfiguration : LowkeyVaultContainerTest
    {
        protected override TokenCredential GetTokenCredential()
        {
            return new NoopCredential();
        }

        private sealed class NoopCredential : TokenCredential
        {
            public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                return new AccessToken("noop", DateTimeOffset.UtcNow.AddHours(1));
            }

            public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
            {
                return new ValueTask<AccessToken>(GetToken(requestContext, cancellationToken));
            }
        }
    }
}