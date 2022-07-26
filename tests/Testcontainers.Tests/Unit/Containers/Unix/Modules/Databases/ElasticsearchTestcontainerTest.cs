namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix.Modules.Databases
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class ElasticsearchTestcontainerTest : IClassFixture<ElasticsearchFixture>
  {
    private readonly ElasticsearchFixture elasticsearchFixture;

    public ElasticsearchTestcontainerTest(ElasticsearchFixture elasticsearchFixture)
    {
      this.elasticsearchFixture = elasticsearchFixture;
    }

    [Fact]
    [Trait("Category", "Elasticsearch")]
    public async Task ConnectionEstablished()
    {
      // Given
      var connection = this.elasticsearchFixture.Connection;

      // When
      var result = await connection.InfoAsync()
        .ConfigureAwait(false);

      // Then
      Assert.True(result.IsValid);
    }
  }
}
