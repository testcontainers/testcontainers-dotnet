namespace Testcontainers.Qdrant;

public sealed class QdrantContainerApiKeyCertificateTest : IAsyncLifetime
{
    private const string Host = "Testcontainers";
    private const string ApiKey = "password!";
    private static readonly PemCertificate Cert = PemCertificate.Create(Host);

    private readonly QdrantContainer _qdrantContainer = new QdrantBuilder()
        .WithApiKey(ApiKey)
        .WithCertificate(Cert.Certificate, Cert.PrivateKey)
        .Build();

    public Task InitializeAsync()
    {
        return _qdrantContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _qdrantContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListCollectionsReturnsValidResponse()
    {
        var httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                CertificateValidation.Thumbprint(Cert.Thumbprint),
        };

        var channel = GrpcChannel.ForAddress(
            _qdrantContainer.GetGrpcConnectionString(),
            new GrpcChannelOptions
            {
                HttpClient = new HttpClient(httpMessageHandler)
                {
                    DefaultRequestHeaders = { Host = Host },
                },
            });
        var callInvoker = channel.Intercept(metadata =>
        {
            metadata.Add("api-key", ApiKey);
            return metadata;
        });

        var grpcClient = new QdrantGrpcClient(callInvoker);
        var client = new QdrantClient(grpcClient);
        var response = await client.ListCollectionsAsync();
        
        Assert.Empty(response);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListCollectionsWithoutApiKeyReturnsInvalidResponse()
    {
        var httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                CertificateValidation.Thumbprint(Cert.Thumbprint)
        };

        var channel = GrpcChannel.ForAddress(
            _qdrantContainer.GetGrpcConnectionString(),
            new GrpcChannelOptions
            {
                HttpClient = new HttpClient(httpMessageHandler)
                {
                    DefaultRequestHeaders = { Host = Host },
                },
            });

        var grpcClient = new QdrantGrpcClient(channel);
        var client = new QdrantClient(grpcClient);
        
        var exception = await Assert.ThrowsAsync<RpcException>(async () => 
            await client.ListCollectionsAsync());
        
        Assert.Equal(StatusCode.PermissionDenied, exception.Status.StatusCode);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListCollectionsWithoutCertificateValidationReturnsInvalidResponse()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString()),
            DefaultRequestHeaders = { Host = Host },
        };
        
        client.DefaultRequestHeaders.Add("api-key", ApiKey);
        
        // The SSL connection could not be established
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("/collections"));
    }
}