namespace Testcontainers.Azurite.Tests.Unit
{
  public sealed class CustomWorkspaceBinding : IClassFixture<AzuriteWithWorkspaceBindingFixture>
  {
    private readonly IEnumerable<string> dataFiles;

    public CustomWorkspaceBinding(AzuriteWithWorkspaceBindingFixture azurite)
    {
      var workspaceLocation = azurite.Container.WorkspaceLocationBinding;
      this.dataFiles = Directory.Exists(workspaceLocation)
        ? Directory
          .EnumerateFiles(workspaceLocation, "*", SearchOption.TopDirectoryOnly)
          .Select(Path.GetFileName)
        : Array.Empty<string>();
    }

    [Fact]
    public void ShouldGetDataFiles()
    {
      Assert.Contains(AzuriteDataFileNames.BlobServiceDataFileName, this.dataFiles);
      Assert.Contains(AzuriteDataFileNames.QueueServiceDataFileName, this.dataFiles);
      Assert.Contains(AzuriteDataFileNames.TableServiceDataFileName, this.dataFiles);
    }
  }
}
