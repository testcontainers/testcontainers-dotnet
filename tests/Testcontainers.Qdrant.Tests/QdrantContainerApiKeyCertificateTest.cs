using System.Net.Http;
using System.Security.Cryptography.X509Certificates;

namespace Testcontainers.Qdrant;

public sealed class QdrantContainerApiKeyCertificateTest : IAsyncLifetime
{
    private static readonly X509Certificate2 Cert = X509CertificateGenerator.GenerateCert("CN=Testcontainers");
    private const string ApiKey = "password!";

    private readonly QdrantContainer _qdrantContainer = new QdrantBuilder()
        .WithApiKey(ApiKey)
        .WithCertificate(Cert)
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
    public async Task PingReturnsValidResponse()
    {
        var httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, certificate, _, _) => 
                certificate.Thumbprint == Cert.Thumbprint,
        };
        
        var client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString()),
            DefaultRequestHeaders = { Host = "Testcontainers" },
        };
        
        client.DefaultRequestHeaders.Add("api-key", ApiKey);

        var response = await client.GetAsync("/collections");
        
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingWithoutApiKeyReturnsInvalidResponse()
    {
        var httpMessageHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
        };
        
        var client = new HttpClient(httpMessageHandler)
        {
            BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString()),
            DefaultRequestHeaders = { Host = "Testcontainers" },
        };
        
        var response = await client.GetAsync("/collections");
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal("Invalid api-key", await response.Content.ReadAsStringAsync());
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingWithoutCertificateValidationReturnsInvalidResponse()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString()),
            DefaultRequestHeaders = { Host = "Testcontainers" },
        };
        
        client.DefaultRequestHeaders.Add("api-key", ApiKey);
        
        // The SSL connection could not be established
        await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync("/collections"));
    }
}