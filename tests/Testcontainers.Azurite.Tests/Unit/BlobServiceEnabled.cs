namespace Testcontainers.Azurite.Tests.Unit;

public sealed class BlobServiceEnabled : IClassFixture<AzuriteWithBlobOnlyFixture>
{
  private readonly AzuriteDefaultFixture _azurite;

  public BlobServiceEnabled(AzuriteWithBlobOnlyFixture azurite)
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
    var blobProperties = await blobServiceClient.GetPropertiesAsync()
      .ConfigureAwait(false);

    var execResult = await _azurite.Container.ExecAsync(new List<string> {"ls", AzuriteBuilder.DefaultWorkspaceDirectoryPath})
      .ConfigureAwait(false);

    // Then
    Assert.False(blobProperties.IsError());
    Assert.Equal(0, execResult.ExitCode);
    Assert.Contains(AzuriteDataFileNames.BlobServiceDataFileName, execResult.Stdout);

    Assert.DoesNotContain(AzuriteDataFileNames.QueueServiceDataFileName, execResult.Stdout);
    Assert.DoesNotContain(AzuriteDataFileNames.TableServiceDataFileName, execResult.Stdout);

    await Assert.ThrowsAsync<RequestFailedException>(() => queueServiceClient.GetPropertiesAsync())
      .ConfigureAwait(false);

    await Assert.ThrowsAsync<RequestFailedException>(() => tableServiceClient.GetPropertiesAsync())
      .ConfigureAwait(false);
  }
}