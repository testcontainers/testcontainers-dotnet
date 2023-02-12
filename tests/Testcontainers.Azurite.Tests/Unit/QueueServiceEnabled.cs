namespace Testcontainers.Azurite.Tests.Unit;

public sealed class QueueServiceEnabled : IClassFixture<AzuriteWithQueueOnlyFixture>
{
  private readonly AzuriteDefaultFixture _azurite;

  public QueueServiceEnabled(AzuriteWithQueueOnlyFixture azurite)
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
    var queueProperties = await queueServiceClient.GetPropertiesAsync()
      .ConfigureAwait(false);

    var execResult = await _azurite.Container.ExecAsync(new List<string> {"ls", AzuriteBuilder.DefaultWorkspaceDirectoryPath})
      .ConfigureAwait(false);

    // Then
    Assert.False(queueProperties.IsError());
    Assert.Equal(0, execResult.ExitCode);
    Assert.Contains(AzuriteDataFileNames.QueueServiceDataFileName, execResult.Stdout);

    Assert.DoesNotContain(AzuriteDataFileNames.BlobServiceDataFileName, execResult.Stdout);
    Assert.DoesNotContain(AzuriteDataFileNames.TableServiceDataFileName, execResult.Stdout);

    await Assert.ThrowsAsync<RequestFailedException>(() => blobServiceClient.GetPropertiesAsync())
      .ConfigureAwait(false);

    await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync())
      .ConfigureAwait(false);
  }
}