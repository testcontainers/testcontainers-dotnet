using DotNet.Testcontainers.Configurations;

using FluentAssertions;

using System.Threading;
using System.Threading.Tasks;

using Testcontainers.Qdrant.Tests.QdrantClient;

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

  public TestOfQdrantContainerWithVolumeMountedConfiguration(ITestOutputHelper output)
  {
    this.output = output;
    quadrantContainer = new QdrantBuilder()
        .WithContainerName("qdrant-testcontainer")
        .WithVolume("/temp/testcontainer_qdrant_storage", "/qdrant/storage", AccessMode.ReadWrite)
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
    var cancellationToken = CancellationToken.None;
    var url = quadrantContainer.GetConnectionUrl();
    var vectorParams = new VectorParams(VectorSize, Distance.DOT, true);
    IQdrantVectorDbClient client = new QdrantVectorDbClient(url);

    var response = await client.CreateCollection(CollectionName, vectorParams, cancellationToken);
    response.Switch(
        _ => output.WriteLine($"Created Collection {CollectionName}"),
        error => throw new QdrantException(error.Error)
    );
    var result = await client.GetCollection(CollectionName, cancellationToken);
    result.Switch(
        collectionInfo =>
        {
          output.WriteLine($"Found Collection {CollectionName}");
          collectionInfo.Should().NotBeNull();
        },
        error => throw new QdrantException(error.Error)
    );
  }
}
