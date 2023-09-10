using System.IO;
using System.Net.Http;

namespace Testcontainers.Qdrant;

public sealed class QdrantContainerConfigurationFileTest : IAsyncLifetime
{
    private const string ApiKey = "password!";
    
    private readonly QdrantContainer _qdrantContainer = new QdrantBuilder()
        .WithConfigFile(CreateConfigFile())
        .Build();

    private static string CreateConfigFile()
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllLines(tempFile, new[]
        {
            "service:",
            $"  api_key: {ApiKey}",
        });
        return tempFile;
    }

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
        var client = new HttpClient
        {
            BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString()),
        };
        
        client.DefaultRequestHeaders.Add("api-key", ApiKey);

        var response = await client.GetAsync("/collections");
        Assert.True(response.IsSuccessStatusCode);
    }
    
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingWithoutApiKeyReturnsInvalidResponse()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri(_qdrantContainer.GetHttpConnectionString()),
        };
        
        var response = await client.GetAsync("/collections");
        
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal("Invalid api-key", await response.Content.ReadAsStringAsync());
    }
}