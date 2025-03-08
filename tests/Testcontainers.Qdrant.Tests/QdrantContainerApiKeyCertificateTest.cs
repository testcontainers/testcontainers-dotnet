namespace Testcontainers.Qdrant;

public sealed class QdrantContainerApiKeyCertificateTest : IAsyncLifetime
{
    // # --8<-- [start:UseQdrantContainer]
    private const string Host = "Testcontainers";
    private const string ApiKey = "password!";
    private static readonly PemCertificate Cert = PemCertificate.Create(Host);

    private readonly QdrantContainer _qdrant = new QdrantBuilder()
        .WithApiKey(ApiKey)
        .WithCertificate(Cert.Certificate, Cert.PrivateKey)
        .Build();

    public Task InitializeAsync()
    {
        return _qdrant.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _qdrant.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthReturnsValidResponse()
    {
        var httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                CertificateValidation.Thumbprint(Cert.Thumbprint),
        };

        var channel = GrpcChannel.ForAddress(
            _qdrant.GetGrpcConnectionString(),
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
        var response = await client.HealthAsync();
        
        Assert.NotEmpty(response.Title);
    }
    // # --8<-- [end:UseQdrantContainer]
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthWithoutApiKeyReturnsInvalidResponse()
    {
        var httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = 
                CertificateValidation.Thumbprint(Cert.Thumbprint)
        };

        var channel = GrpcChannel.ForAddress(
            _qdrant.GetGrpcConnectionString(),
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
            await client.HealthAsync());
        
        Assert.Equal(StatusCode.Unauthenticated, exception.Status.StatusCode);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task HealthWithoutCertificateValidationReturnsInvalidResponse()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_qdrant.GetHttpConnectionString()),
            DefaultRequestHeaders = { Host = Host },
        };
        
        client.DefaultRequestHeaders.Add("api-key", ApiKey);
        
        // The SSL connection could not be established
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("/"));
    }
}