namespace Testcontainers.Azurite;

public abstract class AzuriteContainerTest : IAsyncLifetime
{
    private static readonly string CertificateFilePath = Certificates.Instance.GetFilePath("server", "server.crt");

    private static readonly string CertificateKeyFilePath = Certificates.Instance.GetFilePath("server", "server.key");

    private static readonly string CaCertificateFilePath = Certificates.Instance.GetFilePath("ca", "ca.crt");

    private readonly AzuriteContainer _azuriteContainer;

    private readonly string _scheme;

    private readonly X509Certificate2 _caCertificate;

    private readonly HttpClientTransport _transport;

    private AzuriteContainerTest(AzuriteContainer azuriteContainer, bool tlsEnabled = false)
    {
        _azuriteContainer = azuriteContainer;
        _scheme = tlsEnabled ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;

        // # --8<-- [start:ConfigureAzuriteClientCertificate]
        _caCertificate = X509CertificateLoader.LoadCertificateFromFile(CaCertificateFilePath);

        var httpMessageHandler = new SocketsHttpHandler();
        httpMessageHandler.SslOptions.CertificateChainPolicy = new X509ChainPolicy();
        httpMessageHandler.SslOptions.CertificateChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        httpMessageHandler.SslOptions.CertificateChainPolicy.CustomTrustStore.Add(_caCertificate);
        httpMessageHandler.SslOptions.CertificateChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

        _transport = new HttpClientTransport(httpMessageHandler);
        // # --8<-- [end:ConfigureAzuriteClientCertificate]
    }

    // # --8<-- [start:UseAzuriteContainer]
    public async ValueTask InitializeAsync()
    {
        await _azuriteContainer.StartAsync()
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
    public async Task EstablishesBlobServiceConnection()
    {
        // Given
        var options = new BlobClientOptions(BlobClientOptions.ServiceVersion.V2025_11_05);
        options.Transport = _transport;

        var client = new BlobServiceClient(_azuriteContainer.GetConnectionString(), options);

        // When
        var properties = await client.GetPropertiesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.False(HasError(properties));
        Assert.Equal(_scheme, new Uri(_azuriteContainer.GetBlobEndpoint()).Scheme);
        Assert.Equal(_azuriteContainer.GetConnectionString(), _azuriteContainer.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesQueueServiceConnection()
    {
        // Given
        var options = new QueueClientOptions(QueueClientOptions.ServiceVersion.V2025_11_05);
        options.Transport = _transport;

        var client = new QueueServiceClient(_azuriteContainer.GetConnectionString(), options);

        // When
        var properties = await client.GetPropertiesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.False(HasError(properties));
        Assert.Equal(_scheme, new Uri(_azuriteContainer.GetQueueEndpoint()).Scheme);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesTableServiceConnection()
    {
        // Given
        var options = new TableClientOptions();
        options.Transport = _transport;

        var client = new TableServiceClient(_azuriteContainer.GetConnectionString(), options);

        // When
        var properties = await client.GetPropertiesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.False(HasError(properties));
        Assert.Equal(_scheme, new Uri(_azuriteContainer.GetTableEndpoint()).Scheme);
    }
    // # --8<-- [end:UseAzuriteContainer]

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await _azuriteContainer.DisposeAsync()
            .ConfigureAwait(false);

        _transport.Dispose();
        _caCertificate.Dispose();
    }

    private static bool HasError<TResponseEntity>(NullableResponse<TResponseEntity> response)
    {
        using (var rawResponse = response.GetRawResponse())
        {
            return rawResponse.IsError;
        }
    }

    // # --8<-- [start:CreateAzuriteContainer]
    [UsedImplicitly]
    public sealed class AzuriteDefaultConfiguration : AzuriteContainerTest
    {
        public AzuriteDefaultConfiguration()
            : base(new AzuriteBuilder(TestSession.GetImageFromDockerfile()).Build())
        {
        }
    }
    // # --8<-- [end:CreateAzuriteContainer]

    // # --8<-- [start:ConfigureAzuriteContainerCertificate]
    [UsedImplicitly]
    public sealed class AzuriteSslConfiguration : AzuriteContainerTest
    {
        public AzuriteSslConfiguration()
            : base(new AzuriteBuilder(TestSession.GetImageFromDockerfile()).WithSsl(CertificateFilePath, CertificateKeyFilePath).Build(), true)
        {
        }
    }
    // # --8<-- [end:ConfigureAzuriteContainerCertificate]

    // # --8<-- [start:InMemoryContainerConfiguration]
    [UsedImplicitly]
    public sealed class AzuriteInMemoryConfiguration : AzuriteContainerTest
    {
        public AzuriteInMemoryConfiguration()
            : base(new AzuriteBuilder(TestSession.GetImageFromDockerfile()).WithInMemoryPersistence().Build())
        {
        }
    }
    // # --8<-- [end:InMemoryContainerConfiguration]

    [UsedImplicitly]
    public sealed class AzuriteMemoryLimitConfiguration : AzuriteContainerTest
    {
        private const int MemoryLimitInMb = 64;

        private static readonly string[] LineEndings = { "\r\n", "\n" };

        public AzuriteMemoryLimitConfiguration()
            : base(new AzuriteBuilder(TestSession.GetImageFromDockerfile()).WithInMemoryPersistence(MemoryLimitInMb).Build())
        {
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task MemoryLimitIsConfigured()
        {
            // Given
            var (stdout, _) = await _azuriteContainer.GetLogsAsync(timestampsEnabled: false, ct: TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // When
            var firstLine = stdout.Split(LineEndings, StringSplitOptions.RemoveEmptyEntries).First();

            // Then
            Assert.StartsWith(string.Format(CultureInfo.InvariantCulture, "In-memory extent storage is enabled with a limit of {0:F2} MB", MemoryLimitInMb), firstLine);
        }
    }
}