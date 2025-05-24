using JetBrains.Annotations;
using OpenSearch.Net;

namespace Testcontainers.OpenSearch;

public abstract partial class OpenSearchContainerTest : IAsyncLifetime
{
    protected readonly OpenSearchContainer _opensearchContainer = new OpenSearchBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _opensearchContainer.StartAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _opensearchContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void PingReturnsValidResponse()
    {
        // Given
        OpenSearchClient client = CreateOpenSearchClient();

        // When
        var response = client.Ping();

        // Then
        Assert.True(response.IsValid);
    }

    protected abstract OpenSearchClient CreateOpenSearchClient();

    private const string indexName = "testcontainers";

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldCreateIndexAndAlias()
    {
        // Given
        OpenSearchClient client = CreateOpenSearchClient();

        // When
        CreateIndexResponse createIndexResponse = await CreateTestIndex(client, indexName);

        var createIndexAliasResponse = await client.Indices.PutAliasAsync(
            Indices.Index(indexName),
            new Name($"{indexName}-alias"),
            ct: TestContext.Current.CancellationToken);

        // Then
        Assert.True(createIndexResponse.IsValid);

        Assert.True(createIndexAliasResponse.IsValid);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldIndexAndSearchForDocument()
    {
        // Given
        OpenSearchClient client = CreateOpenSearchClient();

        // When
        CreateIndexResponse createIndexResponse = await CreateTestIndex(client, indexName);

        var doc = new OpenSearchContainerTestDocument()
        {
            DocId = 100,
            Title = "testcontainers-1",
            Text = "some long text"
        };
        var indexDocumentResponse = await client.IndexAsync(
            doc,
            s => s
                .Index(indexName)
                .Id(doc.DocId) // set document id explicitly
                .Refresh(Refresh.True), // allows to search for this doc immideately
            TestContext.Current.CancellationToken);

        var searchResponse = await client.SearchAsync<OpenSearchContainerTestDocument>(
            s => s
                .Index(indexName)
                .Query(q => q.Match(
                        s => s
                            .Field("title")
                            .Query(doc.Title)
                    )
                ),
            TestContext.Current.CancellationToken);

        // Then
        Assert.True(createIndexResponse.IsValid);

        Assert.True(indexDocumentResponse.IsValid);
        Assert.Equal(doc.DocId.ToString(), indexDocumentResponse.Id);

        Assert.True(searchResponse.IsValid);
        Assert.Single(searchResponse.Documents, c => c.DocId == doc.DocId);
    }

    private static async Task<CreateIndexResponse> CreateTestIndex(OpenSearchClient client, string indexName = "testcontainers")
    {
        return await client.Indices.CreateAsync(Indices.Index(indexName),
            c => c
                .Settings(
                    s => s
                    .NumberOfReplicas(0)
                    .NumberOfShards(1)
                )
                .Map<OpenSearchContainerTestDocument>(
                    m => m.AutoMap()),
            TestContext.Current.CancellationToken
        );
    }
}

public partial class OpenSearchContainerTest
{
    internal class OpenSearchContainerTestDocument
    {
        public long DocId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}

[UsedImplicitly]
public sealed class OpenSearchSslBasicAuth : OpenSearchContainerTest
{
    protected override OpenSearchClient CreateOpenSearchClient()
    {
        var credentials = _opensearchContainer.GetConnectionCredentials();
        var clientSettings = new ConnectionSettings(new Uri(_opensearchContainer.GetConnection()))
            .DisableDirectStreaming() // for debugging
            .BasicAuthentication(credentials.UserName, credentials.SecurePassword)
            .ServerCertificateValidationCallback((_, _, _, _) => true); // self-signed certificate

        return new OpenSearchClient(clientSettings);
    }
}