namespace Testcontainers.Azurite.Tests.Unit;

public sealed class TableServiceEnabled : IClassFixture<AzuriteWithTableOnlyFixture>
{
  private readonly AzuriteDefaultFixture _azurite;

  public TableServiceEnabled(AzuriteWithTableOnlyFixture azurite)
  {
    _azurite = azurite;
  }

  [Fact]
  public async Task ConnectionEstablished()
  {
    // Given
    var blobServiceClient = new BlobServiceClient(_azurite.Container.ConnectionString);

    var queueServiceClient = new QueueServiceClient(_azurite.Container.ConnectionString);

    var tableServiceClient = new TableServiceClient(_azurite.Container.ConnectionString);

    // When
    var tableProperties = await tableServiceClient.GetPropertiesAsync()
      .ConfigureAwait(false);

    var execResult = await _azurite.Container.ExecAsync(new List<string> {"ls", AzuriteBuilder.DefaultWorkspaceDirectoryPath})
      .ConfigureAwait(false);

    // Then
    Assert.False(tableProperties.IsError());
    Assert.Equal(0, execResult.ExitCode);
    Assert.Contains(AzuriteDataFileNames.TableServiceDataFileName, execResult.Stdout);

    Assert.DoesNotContain(AzuriteDataFileNames.BlobServiceDataFileName, execResult.Stdout);
    Assert.DoesNotContain(AzuriteDataFileNames.QueueServiceDataFileName, execResult.Stdout);

    await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync())
      .ConfigureAwait(false);

    await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync())
      .ConfigureAwait(false);
  }
}