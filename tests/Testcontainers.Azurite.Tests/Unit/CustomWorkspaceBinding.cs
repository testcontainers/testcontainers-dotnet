namespace Testcontainers.Azurite.Tests.Unit;

public sealed class CustomWorkspaceBinding : IClassFixture<AzuriteWithCustomWorkspaceBindingFixture>
{
  private readonly IEnumerable<string> _dataFiles;

  public CustomWorkspaceBinding(AzuriteWithCustomWorkspaceBindingFixture azurite)
  {
    _dataFiles = Directory.Exists(azurite.Configuration.WorkspaceLocationBinding)
      ? Directory
        .EnumerateFiles(azurite.Configuration.WorkspaceLocationBinding, "*", SearchOption.TopDirectoryOnly)
        .Select(Path.GetFileName)
      : Array.Empty<string>();
  }

  [Fact]
  public void ShouldGetDataFiles()
  {
    Assert.Contains(AzuriteDataFileNames.BlobServiceDataFileName, _dataFiles);
    Assert.Contains(AzuriteDataFileNames.QueueServiceDataFileName, _dataFiles);
    Assert.Contains(AzuriteDataFileNames.TableServiceDataFileName, _dataFiles);
  }
}