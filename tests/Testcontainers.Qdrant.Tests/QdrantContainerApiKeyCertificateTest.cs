using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Qdrant.Client;
using Qdrant.Client.Grpc;
using static Testcontainers.Qdrant.X509CertificateGenerator;
using Uri = System.Uri;

namespace Testcontainers.Qdrant;

public sealed class QdrantContainerApiKeyCertificateTest : IAsyncLifetime
{
    private static readonly PemCertificate Cert = Generate("CN=Testcontainers");
    private static readonly string Thumbprint =
        X509Certificate2.CreateFromPem(Cert.Certificate, Cert.PrivateKey)
            .GetCertHashString(HashAlgorithmName.SHA256);
    private const string ApiKey = "password!";

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
                CertificateValidation.Thumbprint(Thumbprint),
        };

        var channel = GrpcChannel.ForAddress(
            _qdrantContainer.GetGrpcConnectionString(),
            new GrpcChannelOptions
            {
                HttpClient = new HttpClient(httpMessageHandler)
                {
                    DefaultRequestHeaders = { Host = "Testcontainers" },
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
                CertificateValidation.Thumbprint(Thumbprint)
        };

        var channel = GrpcChannel.ForAddress(
            _qdrantContainer.GetGrpcConnectionString(),
            new GrpcChannelOptions
            {
                HttpClient = new HttpClient(httpMessageHandler)
                {
                    DefaultRequestHeaders = { Host = "Testcontainers" },
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
            DefaultRequestHeaders = { Host = "Testcontainers" },
        };
        
        client.DefaultRequestHeaders.Add("api-key", ApiKey);
        
        // The SSL connection could not be established
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            client.GetAsync("/collections"));
    }
}