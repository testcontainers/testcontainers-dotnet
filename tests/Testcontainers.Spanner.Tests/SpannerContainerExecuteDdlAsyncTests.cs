using Google.Cloud.Spanner.Data;

namespace Testcontainers.Spanner.Tests;

public class SpannerContainerExecuteDdlAsyncTests
{


  [Fact]
  public async Task GivenContainerIsStartedWhenDdlExecutedThenInsertSucceeds()
  {
    // Arrange
    var builder = new SpannerBuilder();
    await using (var containerManager = builder.Build())
    {
      await containerManager.StartAsync();

      // Act
      await containerManager.ExecuteDdlAsync("CREATE TABLE MyTable( MyTableId STRING(64) NOT NULL, Name STRING(50) NOT NULL) PRIMARY KEY (MyTableId)");

      // Assert
      await using (var connection = new SpannerConnection(containerManager.ConnectionString))
      {
        await connection.OpenAsync();
        var dml = connection.CreateDmlCommand("Insert into MyTable (MyTableId, Name ) Values ('a_Id', 'a_Name')");
        Assert.Equal(1, await dml.ExecuteNonQueryAsync());
      }
    }
  }
}
