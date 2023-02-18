namespace Testcontainers.Azurite.Tests
{
  public sealed class AzuriteCustomWorkspaceBinding : IClassFixture<AzuriteWithWorkspaceBindingFixture>
  {
    private readonly IEnumerable<string> dataFiles;

    public AzuriteCustomWorkspaceBinding(AzuriteWithWorkspaceBindingFixture azurite)
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
