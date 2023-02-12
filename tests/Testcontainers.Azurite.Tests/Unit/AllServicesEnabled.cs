namespace Testcontainers.Azurite.Tests.Unit;

[UsedImplicitly]
public sealed class AllServicesEnabled
{
  private static async Task EstablishConnection(AzuriteDefaultFixture azurite)
  {
    // Given
    var blobServiceClient = new BlobServiceClient(azurite.Container.ConnectionString);

    var queueServiceClient = new QueueServiceClient(azurite.Container.ConnectionString);

    var tableServiceClient = new TableServiceClient(azurite.Container.ConnectionString);

    // When
    var blobProperties = await blobServiceClient.GetPropertiesAsync()
      .ConfigureAwait(false);

    var queueProperties = await queueServiceClient.GetPropertiesAsync()
      .ConfigureAwait(false);

    var tableProperties = await tableServiceClient.GetPropertiesAsync()
      .ConfigureAwait(false);

    var execResult = await azurite.Container
      .ExecAsync(new List<string> {"ls", AzuriteBuilder.DefaultWorkspaceDirectoryPath})
      .ConfigureAwait(false);

    // Then
    Assert.False(blobProperties.IsError());
    Assert.False(queueProperties.IsError());
    Assert.False(tableProperties.IsError());
    Assert.Equal(0, execResult.ExitCode);
    Assert.Equal(azurite.Configuration.BlobPort, azurite.Container.BlobContainerPort);
    Assert.Equal(azurite.Configuration.QueuePort, azurite.Container.QueueContainerPort);
    Assert.Equal(azurite.Configuration.TablePort, azurite.Container.TableContainerPort);
    Assert.Contains(AzuriteDataFileNames.BlobServiceDataFileName, execResult.Stdout);
    Assert.Contains(AzuriteDataFileNames.QueueServiceDataFileName, execResult.Stdout);
    Assert.Contains(AzuriteDataFileNames.TableServiceDataFileName, execResult.Stdout);
  }

  public sealed class CommonContainerPorts : IClassFixture<AzuriteDefaultFixture>
  {
    private readonly AzuriteDefaultFixture _commonContainerPorts;

    public CommonContainerPorts(AzuriteDefaultFixture commonContainerPorts)
    {
      _commonContainerPorts = commonContainerPorts;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      var exception = await Record
        .ExceptionAsync(() => EstablishConnection(_commonContainerPorts))
        .ConfigureAwait(false);

      Assert.Null(exception);
    }
  }

  public sealed class CustomContainerPorts : IClassFixture<AzuriteWithCustomContainerPortsFixture>
  {
    private readonly AzuriteDefaultFixture _customContainerPorts;

    public CustomContainerPorts(AzuriteWithCustomContainerPortsFixture customContainerPorts)
    {
      _customContainerPorts = customContainerPorts;
    }

    [Fact]
    public async Task ConnectionEstablished()
    {
      Assert.Null(await Record.ExceptionAsync(() => EstablishConnection(_customContainerPorts))
        .ConfigureAwait(false));
    }
  }
}