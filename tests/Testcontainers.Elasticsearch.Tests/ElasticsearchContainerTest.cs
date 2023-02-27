namespace Testcontainers.Elasticsearch;

public sealed class ElasticsearchContainerTest : IAsyncLifetime
{
    private readonly ElasticsearchContainer _elasticsearchContainer = new ElasticsearchBuilder().Build();

    public Task InitializeAsync()
    {
        return _elasticsearchContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _elasticsearchContainer.DisposeAsync().AsTask();
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
}