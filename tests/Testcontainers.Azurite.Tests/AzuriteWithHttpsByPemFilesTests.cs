namespace Testcontainers.Azurite.Tests
{
  public sealed class AzuriteWithHttpsByPemFilesTests : IClassFixture<AzuriteWithHttpsByPemFilesFixture>
  {
    private readonly AzuriteWithHttpsByPemFilesFixture azuriteFixture;

    public AzuriteWithHttpsByPemFilesTests(AzuriteWithHttpsByPemFilesFixture azuriteFixture)
    {
      this.azuriteFixture = azuriteFixture;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      // Given
      var blobServiceClient = new BlobServiceClient(this.azuriteFixture.Container.ConnectionString, new BlobClientOptions { Transport = AzuriteWithHttpsByPemFilesFixture.HttpClientTransport });

      var queueServiceClient = new QueueServiceClient(this.azuriteFixture.Container.ConnectionString, new QueueClientOptions { Transport = AzuriteWithHttpsByPemFilesFixture.HttpClientTransport });

      var tableServiceClient = new TableServiceClient(this.azuriteFixture.Container.ConnectionString, new TableClientOptions { Transport = AzuriteWithHttpsByPemFilesFixture.HttpClientTransport });

      // When
      var blobProperties = await blobServiceClient.GetPropertiesAsync()
        .ConfigureAwait(false);

      var queueProperties = await queueServiceClient.GetPropertiesAsync()
        .ConfigureAwait(false);

      var tableProperties = await tableServiceClient.GetPropertiesAsync()
        .ConfigureAwait(false);

      var execResult = await this.azuriteFixture.Container
        .ExecAsync(new List<string> { "ls", AzuriteBuilder.DefaultWorkspaceDirectoryPath })
        .ConfigureAwait(false);

      // Then
      Assert.False(blobProperties.IsError());
      Assert.False(queueProperties.IsError());
      Assert.False(tableProperties.IsError());
      Assert.Equal(AzuriteBuilder.DefaultBlobPort, this.azuriteFixture.Container.BlobContainerPort);
      Assert.Equal(AzuriteBuilder.DefaultQueuePort, this.azuriteFixture.Container.QueueContainerPort);
      Assert.Equal(AzuriteBuilder.DefaultTablePort, this.azuriteFixture.Container.TableContainerPort);
      Assert.Contains(AzuriteDataFileNames.BlobServiceDataFileName, execResult.Stdout);
      Assert.Contains(AzuriteDataFileNames.QueueServiceDataFileName, execResult.Stdout);
      Assert.Contains(AzuriteDataFileNames.TableServiceDataFileName, execResult.Stdout);
    }
  }
}
