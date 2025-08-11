namespace Testcontainers.OpenSearch;

// <!-- -8<- [start:BaseClass] -->
public abstract class OpenSearchContainerTest : IAsyncLifetime
{
    private const string IndexName = "testcontainers";

    private readonly OpenSearchContainer _openSearchContainer;

    private OpenSearchContainerTest(OpenSearchContainer openSearchContainer)
    {
        _openSearchContainer = openSearchContainer;
    }

    public async ValueTask InitializeAsync()
    {
        await _openSearchContainer.StartAsync()
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore()
            .ConfigureAwait(false);

        GC.SuppressFinalize(this);
    }
    // <!-- -8<- [end:BaseClass] -->

    // <!-- -8<- [start:PingExample] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingReturnsValidResponse()
    {
        // Given
        var client = CreateClient();

        // When
        var response = await client.PingAsync(ct: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(response.IsValid);
    }
    // <!-- -8<- [end:PingExample] -->

    // <!-- -8<- [start:CreateIndexAndAlias] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldCreateIndexAndAlias()
    {
        // Given
        var client = CreateClient();

        var index = Indices.Index(IndexName);

        var alias = new Name(IndexName + "-alias");

        // When
        var createIndexResponse = await CreateIndexAsync(client)
            .ConfigureAwait(true);

        var createAliasResponse = await client.Indices.PutAliasAsync(index, alias, ct: TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(createIndexResponse.IsValid);
        Assert.True(createAliasResponse.IsValid);
    }
    // <!-- -8<- [end:CreateIndexAndAlias] -->

    // <!-- -8<- [start:IndexingDocument] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ShouldIndexAndSearchForDocument()
    {
        // Given
        var client = CreateClient();

        var document = new Document(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        // When
        Func<IndexDescriptor<Document>, IIndexRequest<Document>> indexRequest = i =>
            i.Index(IndexName).Id(document.Id).Refresh(Refresh.True);

        Func<SearchDescriptor<Document>, ISearchRequest> searchRequest = s =>
            s.Index(IndexName).Query(q => q.Match(m => m.Field("title").Query(document.Title)));

        var createIndexResponse = await CreateIndexAsync(client)
            .ConfigureAwait(true);

        var indexResponse = await client.IndexAsync(document, indexRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var searchResponse = await client.SearchAsync(searchRequest, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(createIndexResponse.IsValid);

        Assert.True(indexResponse.IsValid);
        Assert.Equal(document.Id, indexResponse.Id);

        Assert.True(searchResponse.IsValid);
        Assert.Single(searchResponse.Documents, item => document.Id.Equals(item.Id));
    }
    // <!-- -8<- [end:IndexingDocument] -->

    // <!-- -8<- [start:CreateIndexImplementation] -->
    private static Task<CreateIndexResponse> CreateIndexAsync(OpenSearchClient client)
    {
        Func<CreateIndexDescriptor, ICreateIndexRequest> createIndexRequest = c =>
            c.Settings(s => s.NumberOfReplicas(0).NumberOfShards(1)).Map<Document>(m => m.AutoMap());

        return client.Indices.CreateAsync(Indices.Index(IndexName), createIndexRequest, TestContext.Current.CancellationToken);
    }
    // <!-- -8<- [end:CreateIndexImplementation] -->

    protected virtual ValueTask DisposeAsyncCore()
    {
        return _openSearchContainer.DisposeAsync();
    }

    protected virtual OpenSearchClient CreateClient()
    {
        var credentials = _openSearchContainer.GetCredentials();

        var connectionString = new Uri(_openSearchContainer.GetConnectionString());
        Assert.Equal(Uri.UriSchemeHttps, connectionString.Scheme);

        var connectionSettings = new ConnectionSettings(connectionString)
            .BasicAuthentication(credentials.UserName, credentials.Password)
            .ServerCertificateValidationCallback((_, _, _, _) => true);

        return new OpenSearchClient(connectionSettings);
    }

    // <!-- -8<- [start:InsecureNoAuth] -->
    [UsedImplicitly]
    public sealed class InsecureNoAuthConfiguration : OpenSearchContainerTest
    {
        public InsecureNoAuthConfiguration()
            : base(new OpenSearchBuilder()
                .WithSecurityEnabled(false)
                .Build())
        {
        }

        protected override OpenSearchClient CreateClient()
        {
            var connectionString = new Uri(_openSearchContainer.GetConnectionString());
            Assert.Equal(Uri.UriSchemeHttp, connectionString.Scheme);
            return new OpenSearchClient(connectionString);
        }
    }
    // <!-- -8<- [end:InsecureNoAuth] -->

    // <!-- -8<- [start:SslBasicAuthDefaultCredentials] -->
    [UsedImplicitly]
    public sealed class SslBasicAuthDefaultCredentialsConfiguration : OpenSearchContainerTest
    {
        public SslBasicAuthDefaultCredentialsConfiguration()
            : base(new OpenSearchBuilder()
                .Build())
        {
        }
    }
    // <!-- -8<- [end:SslBasicAuthDefaultCredentials] -->

    // <!-- -8<- [start:SslBasicAuthCustomCredentials] -->
    [UsedImplicitly]
    public sealed class SslBasicAuthCustomCredentialsConfiguration : OpenSearchContainerTest
    {
        public SslBasicAuthCustomCredentialsConfiguration()
            : base(new OpenSearchBuilder()
                .WithPassword(new string(OpenSearchBuilder.DefaultPassword.Reverse().ToArray()))
                .Build())
        {
        }
    }
    // <!-- -8<- [end:SslBasicAuthCustomCredentials] -->

    [UsedImplicitly]
    private record Document(string Id, string Title);
}