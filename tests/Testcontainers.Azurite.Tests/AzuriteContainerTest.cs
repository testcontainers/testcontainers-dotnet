namespace Testcontainers.Azurite;

public abstract class AzuriteContainerTest : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer;

    private AzuriteContainerTest(AzuriteContainer azuriteContainer)
    {
        _azuriteContainer = azuriteContainer;
    }

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

    // # --8<-- [start:UseAzuriteContainer]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesBlobServiceConnection()
    {
        // Give
        var client = new BlobServiceClient(_azuriteContainer.GetConnectionString(), ConfigureClientOptions(new BlobClientOptions(BlobClientOptions.ServiceVersion.V2025_11_05)));

        // When
        var properties = await client.GetPropertiesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.False(HasError(properties));
        Assert.Equal(_azuriteContainer.GetConnectionString(), _azuriteContainer.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesQueueServiceConnection()
    {
        // Give
        var client = new QueueServiceClient(_azuriteContainer.GetConnectionString(), ConfigureClientOptions(new QueueClientOptions(QueueClientOptions.ServiceVersion.V2025_11_05)));

        // When
        var properties = await client.GetPropertiesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.False(HasError(properties));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task EstablishesTableServiceConnection()
    {
        // Give
        var client = new TableServiceClient(_azuriteContainer.GetConnectionString(), ConfigureClientOptions(new TableClientOptions()));

        // When
        var properties = await client.GetPropertiesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.False(HasError(properties));
    }
    // # --8<-- [end:UseAzuriteContainer]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _azuriteContainer.DisposeAsync();
    }

    protected virtual TClientOptions ConfigureClientOptions<TClientOptions>(TClientOptions clientOptions)
        where TClientOptions : ClientOptions
    {
        return clientOptions;
    }

    private static bool HasError<TResponseEntity>(NullableResponse<TResponseEntity> response)
    {
        using (var rawResponse = response.GetRawResponse())
        {
            return rawResponse.IsError;
        }
    }

    [UsedImplicitly]
    public sealed class AzuriteDefaultConfiguration : AzuriteContainerTest
    {
        public AzuriteDefaultConfiguration()
            : base(new AzuriteBuilder(TestSession.GetImageFromDockerfile()).Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class AzuriteInMemoryConfiguration : AzuriteContainerTest
    {
        public AzuriteInMemoryConfiguration()
            : base(new AzuriteBuilder(TestSession.GetImageFromDockerfile()).WithInMemoryPersistence().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class AzuriteSslConfiguration : AzuriteContainerTest
    {
        private readonly X509Certificate2 _caCertificate;

        private readonly HttpClientTransport _transport;

        public AzuriteSslConfiguration()
            : base(Configure().Build())
        {
            // # --8<-- [start:AzuriteSslClientTransport]
            _caCertificate = X509CertificateLoader.LoadCertificateFromFile(Certificates.Instance.GetFilePath("ca", "ca.crt"));

            var httpMessageHandler = new SocketsHttpHandler();
            httpMessageHandler.SslOptions.CertificateChainPolicy = new X509ChainPolicy();
            httpMessageHandler.SslOptions.CertificateChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
            httpMessageHandler.SslOptions.CertificateChainPolicy.CustomTrustStore.Add(_caCertificate);
            httpMessageHandler.SslOptions.CertificateChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

            _transport = new HttpClientTransport(httpMessageHandler);
            // # --8<-- [end:AzuriteSslClientTransport]
        }

        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public void ConnectionStringUsesHttpsScheme()
        {
            // Given
            var connectionString = _azuriteContainer.GetConnectionString();

            // When
            var endpoints = new[] { _azuriteContainer.GetBlobEndpoint(), _azuriteContainer.GetQueueEndpoint(), _azuriteContainer.GetTableEndpoint() };

            // Then
            Assert.Contains("DefaultEndpointsProtocol=https", connectionString);
            Assert.All(endpoints, endpoint => Assert.StartsWith("https://", endpoint));
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore()
                .ConfigureAwait(false);

            _transport.Dispose();
            _caCertificate.Dispose();
        }

        // # --8<-- [start:AzuriteSslClientOptions]
        protected override TClientOptions ConfigureClientOptions<TClientOptions>(TClientOptions clientOptions)
        {
            clientOptions.Transport = _transport;
            return clientOptions;
        }
        // # --8<-- [end:AzuriteSslClientOptions]

        // # --8<-- [start:AzuriteSslBuilder]
        private static AzuriteBuilder Configure()
            => new AzuriteBuilder(TestSession.GetImageFromDockerfile())
                .WithSsl(Certificates.Instance.GetFilePath("server", "server.crt"), Certificates.Instance.GetFilePath("server", "server.key"));
        // # --8<-- [end:AzuriteSslBuilder]
    }

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