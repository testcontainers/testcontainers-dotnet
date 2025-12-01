namespace Testcontainers.Elasticsearch;

public abstract class ElasticsearchContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseElasticsearchContainer]
    private readonly ElasticsearchContainer _elasticsearchContainer;

    protected ElasticsearchContainerTest(ElasticsearchContainer container)
    {
        _elasticsearchContainer = container;
    }

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
    public void PingReturnsValidResponse()
    {
        // Given
        var clientSettings = new ElasticsearchClientSettings(new Uri(_elasticsearchContainer.GetConnectionString()));
        clientSettings.ServerCertificateValidationCallback(CertificateValidations.AllowAll);

        var client = new ElasticsearchClient(clientSettings);

        // When
        var response = client.Ping();

        // Then
        Assert.True(response.IsValidResponse);
    }
    // # --8<-- [end:UseElasticsearchContainer]

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _elasticsearchContainer.DisposeAsync();
    }

    [UsedImplicitly]
    public sealed class ElasticsearchDefaultConfiguration : ElasticsearchContainerTest
    {
        public ElasticsearchDefaultConfiguration() : base(new ElasticsearchBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class ElasticsearchAuthConfiguration : ElasticsearchContainerTest
    {
        public ElasticsearchAuthConfiguration() : base(new ElasticsearchBuilder().WithPassword("CustomCredentialsConfiguration").Build())
        {
        }
    }
}