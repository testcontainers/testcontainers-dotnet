namespace Testcontainers.Elasticsearch;

public abstract class ElasticsearchContainerTest : IAsyncLifetime
{
    private readonly ElasticsearchContainer _elasticsearchContainer;

    private ElasticsearchContainerTest(ElasticsearchContainer elasticsearchContainer)
    {
        _elasticsearchContainer = elasticsearchContainer;
    }

    // # --8<-- [start:UseElasticsearchContainer]
    public async ValueTask InitializeAsync()
    {
        await _elasticsearchContainer.StartAsync()
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
    public async Task PingReturnsValidResponse()
    {
        // Given
        using var caCertificate = await _elasticsearchContainer.GetCertificateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var connectionString = new Uri(_elasticsearchContainer.GetConnectionString());

        var clientSettings = new ElasticsearchClientSettings(connectionString);
        clientSettings.ServerCertificateValidationCallback(CertificateValidations.AuthorityIsRoot(caCertificate));

        var client = new ElasticsearchClient(clientSettings);

        // When
        var response = await client.PingAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(response.IsValidResponse);
        Assert.Equal(_elasticsearchContainer.GetConnectionString(), _elasticsearchContainer.GetConnectionString(ConnectionMode.Host));
    }
    // # --8<-- [end:UseElasticsearchContainer]

    // # --8<-- [start:UseElasticsearchCertificate]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ClusterHealthReturnsValidResponse()
    {
        // Given
        using var caCertificate = await _elasticsearchContainer.GetCertificateAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        using var httpMessageHandler = new HttpClientHandler();
        httpMessageHandler.ServerCertificateCustomValidationCallback = CertificateValidations.AuthorityIsRoot(caCertificate);

        var connectionString = new Uri(_elasticsearchContainer.GetConnectionString());

        var authenticationHeaderValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(Uri.UnescapeDataString(connectionString.UserInfo))));

        using var httpClient = new HttpClient(httpMessageHandler);
        httpClient.BaseAddress = connectionString;
        httpClient.DefaultRequestHeaders.Authorization = authenticationHeaderValue;

        // When
        using var httpResponseMessage = await httpClient.GetAsync("/_cluster/health", TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, httpResponseMessage.StatusCode);
    }
    // # --8<-- [end:UseElasticsearchCertificate]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _elasticsearchContainer.DisposeAsync();
    }

    // # --8<-- [start:CreateElasticsearchContainer]
    [UsedImplicitly]
    public sealed class ElasticsearchDefaultConfiguration : ElasticsearchContainerTest
    {
        public ElasticsearchDefaultConfiguration()
            : base(new ElasticsearchBuilder(TestSession.GetImageFromDockerfile()).Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class ElasticsearchAuthConfiguration : ElasticsearchContainerTest
    {
        public ElasticsearchAuthConfiguration()
            : base(new ElasticsearchBuilder(TestSession.GetImageFromDockerfile()).WithPassword("some-password").Build())
        {
        }
    }
    // # --8<-- [end:CreateElasticsearchContainer]
}