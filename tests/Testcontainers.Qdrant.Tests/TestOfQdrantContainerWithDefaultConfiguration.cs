using QdrantCSharp;
using QdrantCSharp.Enums;
using QdrantCSharp.Models;

using System.Threading.Tasks;

using Xunit.Abstractions;

namespace Testcontainers.Qdrant.Tests
{
  /// <summary>
  /// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
  /// </summary>
  public sealed class TestOfQdrantContainerWithDefaultConfiguration : IAsyncLifetime
  {
    private readonly ITestOutputHelper output;
    private readonly QdrantContainer quadrantContainer;

    private const string CollectionName = "qdrant-testcontainer-collection";
    private const int VectorSize = 4;

    public TestOfQdrantContainerWithDefaultConfiguration(ITestOutputHelper output)
    {
      this.output = output;
      quadrantContainer = new QdrantBuilder().Build();
    }

    public Task InitializeAsync()
    {
      return quadrantContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
      return quadrantContainer.DisposeAsync().AsTask();
    }


    [Fact]
    public async Task CreateCollectionAndFindIt_UsingDefaultOptions()
    {
      var client = new QdrantHttpClient(quadrantContainer.GetConnectionUrl(), string.Empty);
      var response = await client.CreateCollection(CollectionName, new VectorParams(size: VectorSize, distance: Distance.DOT));
      Assert.True(response.Result);
      output.WriteLine($"Created Collection {CollectionName}");
      var result = await client.GetCollection(CollectionName);
      Assert.NotNull(result);
      Assert.Equal("ok", result.Status);
      output.WriteLine($"Verified Collection {CollectionName}");
    }
  }
}
