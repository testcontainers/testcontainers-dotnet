using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DotNet.Testcontainers.Commons;
using Xunit;

namespace Testcontainers.Weaviate;

public sealed class PostgreSqlContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseWeaviateContainer]
    private readonly WeaviateContainer _weaviateContainer = new WeaviateBuilder().Build();

    public Task InitializeAsync() => _weaviateContainer.StartAsync();
    public Task DisposeAsync() => _weaviateContainer.DisposeAsync().AsTask();

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ListCollections()
    {
        var client = new HttpClient();
        var response = await client.GetAsync(
          $"http://{_weaviateContainer.Hostname}:{_weaviateContainer.GetMappedPublicPort(WeaviateBuilder.WeaviateHttpPort)}/v1/schema");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    // # --8<-- [end:UseWeaviateContainer]
}
