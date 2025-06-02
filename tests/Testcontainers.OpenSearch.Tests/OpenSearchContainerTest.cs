namespace Testcontainers.OpenSearch;

// <!-- -8<- [start:BaseClass] -->
public abstract class OpenSearchContainerTest : IAsyncLifetime
{
    private const string INDEX_NAME = "testcontainers";

    protected OpenSearchContainer OpensearchContainer { get; init; }

    protected abstract OpenSearchClient CreateOpenSearchClient();

    protected OpenSearchContainerTest()
    {
        OpensearchContainer = new OpenSearchBuilder().Build(); // by default, OpenSearch uses https and credentials for connections.
    }

    public async ValueTask InitializeAsync()
    {
        await OpensearchContainer.StartAsync(TestContext.Current.CancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await OpensearchContainer.DisposeAsync();
    }
    // <!-- -8<- [end:BaseClass] -->

    // <!-- -8<- [start:PingExample] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingReturnsValidResponse()
    {
        // Given
        OpenSearchClient client = CreateOpenSearchClient();

        // When
        var response = await client.PingAsync(ct: TestContext.Current.CancellationToken);

        // Then
        Assert.True(response.IsValid);
    }
    // <!-- -8<- [end:PingExample] -->

    // <!-- -8<- [start:IndexAndAliasCreation] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldCreateIndexAndAlias()
    {
        // Given
        OpenSearchClient client = CreateOpenSearchClient();

        // When
        CreateIndexResponse createIndexResponse = await CreateTestIndex(client, INDEX_NAME);

        var createIndexAliasResponse = await client.Indices.PutAliasAsync(
            Indices.Index(INDEX_NAME),
            new Name($"{INDEX_NAME}-alias"),
            ct: TestContext.Current.CancellationToken);

        // Then
        Assert.True(createIndexResponse.IsValid);

        Assert.True(createIndexAliasResponse.IsValid);
    }
    // <!-- -8<- [end:IndexAndAliasCreation] -->

    // <!-- -8<- [start:IndexingDocument] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldIndexAndSearchForDocument()
    {
        // Given
        OpenSearchClient client = CreateOpenSearchClient();

        // When
        CreateIndexResponse createIndexResponse = await CreateTestIndex(client, INDEX_NAME);

        var doc = new OpenSearchContainerTestDocument()
        {
            DocId = 100,
            Title = "testcontainers-1",
            Text = "some long text"
        };
        var indexDocumentResponse = await client.IndexAsync(
            doc,
            s => s
                .Index(INDEX_NAME)
                .Id(doc.DocId) // set document id explicitly
                .Refresh(Refresh.True), // allows to search for this doc immideately
            TestContext.Current.CancellationToken);

        var searchResponse = await client.SearchAsync<OpenSearchContainerTestDocument>(
            s => s
                .Index(INDEX_NAME)
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
    // <!-- -8<- [end:IndexingDocument] -->

    // <!-- -8<- [start:CreateTestIndexImpl] -->
    private static async Task<CreateIndexResponse> CreateTestIndex(
        OpenSearchClient client,
        string indexName = "testcontainers")
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
    // <!-- -8<- [end:CreateTestIndexImpl] -->

    private sealed class OpenSearchContainerTestDocument
    {
        public long DocId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}

// <!-- -8<- [start:SslBasicAuth] -->
[UsedImplicitly]
public class OpenSearchSslBasicAuth : OpenSearchContainerTest
{
    protected override OpenSearchClient CreateOpenSearchClient()
    {
        var credentials = OpensearchContainer.GetCredentials();
        var uri = new Uri(OpensearchContainer.GetConnectionString());
        Assert.Equal(Uri.UriSchemeHttps, uri.Scheme);
        var clientSettings = new ConnectionSettings(uri)
            .BasicAuthentication(credentials.UserName, credentials.SecurePassword)
            .ServerCertificateValidationCallback((_, _, _, _) => true); // validate self-signed certificate

        return new OpenSearchClient(clientSettings);
    }
}
// <!-- -8<- [end:SslBasicAuth] -->

// <!-- -8<- [start:SslBasicAuthCustomPassword] -->
[UsedImplicitly]
public sealed class OpenSearchSslBasicAuthCustomPassword : OpenSearchSslBasicAuth
{
    public OpenSearchSslBasicAuthCustomPassword()
    {
        OpensearchContainer = new OpenSearchBuilder()
            .WithPassword(new string(OpenSearchBuilder.DefaultPassword.Reverse().ToArray()))
            .Build();
    }
}
// <!-- -8<- [end:SslBasicAuthCustomPassword] -->

// <!-- -8<- [start:InsecureNoAuth] -->
[UsedImplicitly]
public sealed class OpenSearchInsecureNoAuth : OpenSearchContainerTest
{
    public OpenSearchInsecureNoAuth()
    {
        OpensearchContainer = new OpenSearchBuilder()
            .WithSecurityEnabled(false) // <-- this disables https and auth
            .Build();
    }

    protected override OpenSearchClient CreateOpenSearchClient()
    {
        var uri = new Uri(OpensearchContainer.GetConnectionString());
        Assert.Equal(Uri.UriSchemeHttp, uri.Scheme);
        var clientSettings = new ConnectionSettings(uri);

        return new OpenSearchClient(clientSettings);
    }
}
// <!-- -8<- [end:InsecureNoAuth] -->