namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
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

    [Fact]
    public void CannotSetDatabase()
    {
      var elasticsearch = new ElasticsearchTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => elasticsearch.Database = string.Empty);
    }

    [Fact]
    public void CannotSetUsername()
    {
      var elasticsearch = new ElasticsearchTestcontainerConfiguration();
      Assert.Throws<NotImplementedException>(() => elasticsearch.Username = string.Empty);
    }
  }
}
