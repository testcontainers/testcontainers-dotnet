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

    public ValueTask DisposeAsync()
    {
        return _elasticsearchContainer.DisposeAsync();
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

    public sealed class DefaultConfiguration : ElasticsearchContainerTest
    {
        public DefaultConfiguration() : base(new ElasticsearchBuilder().Build())
        {
        }
    }

    public sealed class CustomCredentialsConfiguration : ElasticsearchContainerTest
    {
        public CustomCredentialsConfiguration() : base(new ElasticsearchBuilder().WithPassword("CustomCredentialsConfiguration").Build())
        {
        }
    }
}