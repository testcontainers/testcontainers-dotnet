using DotNet.Testcontainers.Containers;
using Google.Cloud.Spanner.Data;

namespace Testcontainers.Spanner.Tests;

public class SpannerContainerStartAsyncTests
{
  [Fact]
  public async Task WhenCompleteThenContainerIsRunning()
  {
    // Arrange
    var builder = new SpannerBuilder();
    await using (var containerManager = builder.Build())
    {

      // Act
      await containerManager.StartAsync();
      // Assert

      Assert.Equal(TestcontainersStates.Running, containerManager.State);
    }
  }

  [Fact]
  public async Task WhenCompleteThenContainerHasRandomizedRestPortMapped()
  {
    // Arrange
    var builder = new SpannerBuilder();
    await using (var containerManager = builder.Build())
    {
      // Act
      await containerManager.StartAsync();
      // Assert

      Assert.NotEqual(default, containerManager.RestPort);
      Assert.NotEqual(9020, containerManager.RestPort);
    }
  }

  [Fact]
  public async Task WhenCompleteThenContainerHasRandomizedGrpcPortMapped()
  {
    // Arrange
    var builder = new SpannerBuilder();
    await using (var containerManager = builder.Build())
    {
      // Act
      await containerManager.StartAsync();
      // Assert

      Assert.NotEqual(default, containerManager.GrpcPort);
      Assert.NotEqual(9010, containerManager.GrpcPort);
    }
  }


  [Fact]
  public async Task WhenCompleteThenEnvironmentVariableEmulatorHostSet()
  {
    // Arrange
    var builder = new SpannerBuilder();
    await using (var containerManager = builder.Build())
    {
      // Act
      await containerManager.StartAsync();

      // Assert
      Assert.Equal($"{containerManager.Hostname}:{containerManager.GrpcPort}", Environment.GetEnvironmentVariable("SPANNER_EMULATOR_HOST"));
    }
  }

  [Fact]
  public async Task WhenCompleteThenConnectionStringUseableToConnect()
  {
    // Arrange
    var builder = new SpannerBuilder();
    await using (var containerManager = builder.Build())
    {
      // Act
      await containerManager.StartAsync();

      // Assert
      using (var connection = new SpannerConnection(containerManager.ConnectionString))
      {
        // Assert is not failing in executing, as status on connection will be ok, even if connectionstring is wrong
        await connection.OpenAsync();
        var ddl = connection.CreateDdlCommand(@"CREATE TABLE MyTable
(
  MyTableId STRING(64) NOT NULL,
  Name STRING(50) NOT NULL
)
PRIMARY KEY (MyTableId)");
        ddl.CommandTimeout = 1;
        _ = await ddl.ExecuteNonQueryAsync();
        // Supress git warning having no assert, the assertion is not failing up to here
        Assert.True(true);
      }
    }
  }
}
