using DotNet.Testcontainers.Configurations;

using QdrantCSharp;
using QdrantCSharp.Enums;
using QdrantCSharp.Models;

using System.IO;
using System.Threading.Tasks;

using Xunit.Abstractions;

namespace Testcontainers.Qdrant.Tests;

/// <summary>
/// <a href="https://github.com/testcontainers/testcontainers-dotnet/tree/develop/src">Documentation/src</a>
/// </summary>
public sealed class TestOfQdrantContainerWithVolumeMountedConfiguration : IAsyncLifetime
{
  private readonly ITestOutputHelper output;
  private readonly QdrantContainer quadrantContainer;

  private const string CollectionName = "qdrant-test-collection";
  private const int VectorSize = 4;
  private const string PathName = "./testcontainer_qdrant_storage";
  public TestOfQdrantContainerWithVolumeMountedConfiguration(ITestOutputHelper output)
  {
    var path = Path.GetFullPath(PathName);
    Directory.CreateDirectory(path);
    this.output = output;
    quadrantContainer = new QdrantBuilder()
      .WithContainerName("qdrant-testcontainer")
      .WithVolume(path, "/qdrant/storage", AccessMode.ReadWrite)
      .Build();
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
  public async Task CreateCollectionAndFindIt_UsingMountedVolumeOptions()
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
