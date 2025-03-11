namespace Testcontainers.OpenSearch;

public sealed class OpenSearchContainerTest : IAsyncLifetime
{
    private readonly OpenSearchContainer _opensearchContainer = new OpenSearchBuilder().Build();

    public Task InitializeAsync()
    {
        return _opensearchContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _opensearchContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void PingReturnsValidResponse()
    {
        // Given
        var clientSettings = new ConnectionSettings(new Uri(_opensearchContainer.GetConnection()))
            .BasicAuthentication(OpenSearchBuilder.DefaultUsername, OpenSearchBuilder.DefaultPassword)
            .ServerCertificateValidationCallback((_, _, _, _) => true); // self-signed certificate

        var client = new OpenSearchClient(clientSettings);

        // When
        var response = client.Ping();

        // Then
        Assert.True(response.IsValid);
    }
}