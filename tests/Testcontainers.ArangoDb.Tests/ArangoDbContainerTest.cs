namespace Testcontainers.ArangoDb;

public sealed class ArangoDbContainerTest : IAsyncLifetime
{
    private readonly ArangoDbContainer _arangoDbContainer = new ArangoDbBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _arangoDbContainer.StartAsync();
    }

    public ValueTask DisposeAsync()
    {
        return _arangoDbContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task RetrievesDatabases()
    {
        // Given
        var address = new Uri(_arangoDbContainer.GetTransportAddress());

        using var transport = HttpApiTransport.UsingBasicAuth(address, ArangoDbBuilder.DefaultUsername, ArangoDbBuilder.DefaultPassword);

        using var client = new ArangoDBClient(transport);

        // When
        var response = await client.Database.GetDatabasesAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.Equal(HttpStatusCode.OK, response.Code);
    }
}