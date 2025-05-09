namespace Testcontainers.Elasticsearch;

public sealed class ElasticsearchContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseElasticsearchContainer]
    private readonly ElasticsearchContainer _elasticsearchContainer = new ElasticsearchBuilder().Build();

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
}